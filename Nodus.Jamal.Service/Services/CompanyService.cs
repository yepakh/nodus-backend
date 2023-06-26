using Azure.Core;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nodus.Database.Context;
using Nodus.Database.Context.Constants;
using Nodus.Database.Models.Admin;
using Nodus.Jamal.Service.GrpcClients;
using Nodus.Jamal.Service.Options;
using Nodus.Jamal.Service.Protos;

namespace Nodus.Jamal.Service.Services
{
    public class CompanyService
    {
        private readonly AdminContext _adminContext;
        private readonly EFContextFactory _efContextFactory;
        private readonly MigratorGrpcClient _migratorGrpcService;
        private readonly JamalOptions _jamalOptions;
        private readonly UserService _userService;
        private readonly RoleService _roleService;

        public CompanyService(AdminContext adminContext,
            EFContextFactory contextFactory,
            MigratorGrpcClient migratorGrpcService,
            IOptions<JamalOptions> jamalOptions,
            UserService userService,
            RoleService roleService)
        {
            _adminContext = adminContext;
            _efContextFactory = contextFactory;
            _migratorGrpcService = migratorGrpcService;
            _jamalOptions = jamalOptions.Value;
            _userService = userService;
            _roleService = roleService;
        }

        public async Task<GetCompaniesResponse> GetCompaniesAsync(GetCompaniesRequest request)
        {
            //get companies
            var companies = await _adminContext.Companies
                .Include(company => company.Roles)
                    .ThenInclude(role => role.RoleFeatures)
                .Include(company => company.Roles)
                    .ThenInclude(role => role.Users)
                .OrderBy(company => company.Name)
                .Skip(request.Pagination.Offset)
                .Take(request.Pagination.Limit)
                .Select(company => new CompanyData()
                {
                    Description = company.Description,
                    Id = company.Id,
                    Name = company.Name,
                    TotalAdminsCount = company.Roles
                        .Where(role => role.RoleFeatures
                            .Select(roleFeature => roleFeature.FeatureId)
                            .Contains((int)FeatureNames.WriteUsers))
                        .SelectMany(role => role.Users)
                        .Count()
                })
                .ToListAsync();

            var totalCount = await _adminContext.Companies.CountAsync();

            var response = new GetCompaniesResponse()
            {
                Companies = { companies },
                Pagination = new PaginationResponse()
                {
                    TotalCount = totalCount
                }
            };

            return response;
        }

        public async Task<CompanyData> GetCompanyByIdAsync(int companyId)
        {
            var company = await _adminContext.Companies
                .Include(company => company.Roles)
                    .ThenInclude(role => role.RoleFeatures)
                .Include(company => company.Roles)
                    .ThenInclude(role => role.Users)
                .Select(company => new CompanyData()
                {
                    Description = company.Description,
                    Id = company.Id,
                    Name = company.Name,
                    TotalAdminsCount = company.Roles
                        .Where(role => role.RoleFeatures
                            .Select(roleFeature => roleFeature.FeatureId)
                            .Contains((int)FeatureNames.WriteUsers))
                        .SelectMany(role => role.Users)
                        .Count()
                })
                .FirstAsync(comp => comp.Id == companyId);

            return company;
        }

        public async Task<int> CreateCompanyAsync(CreateCompanyRequest request)
        {
            await ValidateInputData(request.AdminPhoneNumber, request.AdminEmail, request.CompanyName);

            //Add new company to admin DB
            var newCompany = await _adminContext.Companies.AddAsync(new Company()
            {
                Name = request.CompanyName,
                ConnectionString = string.IsNullOrEmpty(request.ConnectionString) ? string.Format(_jamalOptions.CustomerDatabase, request.CompanyName) : request.ConnectionString,
                Description = request.CompanyDescription
            });
            await _adminContext.SaveChangesAsync();

            //call migrator via gRPC and create company DB
            var result = _migratorGrpcService.MigrateOneClient(newCompany.Entity.Id);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                throw new RpcException(new Status(), result.ErrorMessage);
            }

            //Create company admin role
            var roleResult = await _roleService.CreateRoleAsync(new CreateRoleRequest()
            {
                AvaliableFeaturesIds = 
                {
                    Enum.GetValues(typeof(FeatureNames))
                        .Cast<FeatureNames>()
                        .Where(feature => feature != FeatureNames.ManageCompanies)
                        .Select(feature => (int)feature)
                        .ToList()
                },
                Name = "Company admin",
                Description = "Default admin's role that is generated on company creation.",
                CompanyId = newCompany.Entity.Id,
            });

            //Create company admin user
            await _userService.CreateUserAsync(new CreateUserRequest()
            {
                CompanyId = newCompany.Entity.Id,
                Email = request.AdminEmail,
                FirstName = request.AdminFirstName,
                LastName = request.AdminLastName,
                PhoneNumber = request.AdminPhoneNumber,
                RoleId = roleResult.RoleId
            });

            return newCompany.Entity.Id;
        }

        public async Task DeleteCompanyByIdAsync(int companyId)
        {
            var company = await _adminContext.Companies
                .FirstAsync(company => company.Id == companyId);

            var clientContext = await _efContextFactory.GetRequiredContext(company.Id);
            await clientContext.Database.EnsureDeletedAsync();

            var usersToRemove = _adminContext.Users
                .Include(user => user.Role)
                .Where(user => user.Role.CompanyId == companyId)
                .ToList();

            var rolesToRemove = _adminContext.Roles
                .Where(role => role.CompanyId == companyId);

            _adminContext.Users.RemoveRange(usersToRemove);
            _adminContext.Roles.RemoveRange(rolesToRemove);
            _adminContext.Companies.Remove(company);

            await _adminContext.SaveChangesAsync();
        }

        #region Private routine

        private async Task ValidateInputData(string phone, string email, string companyName)
        {
            bool userIsNotUnique = await _adminContext.Users
                .AnyAsync(user => user.Email == email || user.PhoneNumber == phone);

            bool companyIsNotUnique = await _adminContext.Companies.AnyAsync(company => company.Name.ToLower() == companyName.ToLower());

            if (userIsNotUnique || companyIsNotUnique)
            {
                throw new ArgumentException("User or company data is not unique");
            }
        }

        #endregion Private routine
    }
}
