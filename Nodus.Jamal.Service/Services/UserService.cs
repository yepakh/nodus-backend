using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nodus.Database.Context;
using Nodus.Database.Context.Constants;
using Nodus.Database.Models.Admin;
using Nodus.Database.Models.Customer;
using Nodus.Jamal.Service.GrpcClients;
using Nodus.Jamal.Service.Options;
using Nodus.Jamal.Service.Protos;
using System.Data;
using System.Linq.Expressions;
using Role = Nodus.Database.Models.Admin.Role;

namespace Nodus.Jamal.Service.Services
{
    public class UserService
    {
        private readonly AdminContext _adminContext;
        private readonly EFContextFactory _efContextFactory;
        private readonly NotificatorGrpcClient _notificatorGrpcClient;
        private readonly EmailLinkOptions _emailLinkOptions;

        public UserService(
            AdminContext adminContext,
            EFContextFactory efContextFactory,
            NotificatorGrpcClient notificatorGrpcClient,
            IOptions<EmailLinkOptions> emailLinkOptions)
        {
            _adminContext = adminContext;
            _efContextFactory = efContextFactory;
            _notificatorGrpcClient = notificatorGrpcClient;
            _emailLinkOptions = emailLinkOptions.Value;
        }

        public async Task<GetAllCompanyAdminsResponse> GetAllCompanyAdminsAsync(GetAllCompanyAdminsRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            //get admins
            var admins = await _adminContext.Users
                .Include(usr => usr.Role)
                .ThenInclude(role => role.RoleFeatures)
                .Where(usr => usr.IsActive && usr.Role.RoleFeatures.Any(roleFeature => roleFeature.FeatureId == (int)FeatureNames.WriteUsers))
                .OrderBy(admin => admin.Email)
                .Skip(request.Pagination.Offset)
                .Take(request.Pagination.Limit)
                .ToListAsync();

            var totalCount = await _adminContext.Users
                .Where(usr => usr.IsActive && usr.Role.RoleFeatures.Any(roleFeature => roleFeature.FeatureId == (int)FeatureNames.WriteUsers))
                .CountAsync();

            //get additional data from client db
            var adminsDetails = clientContext.UserDetails
                .Where(usr => admins.Select(adm => adm.Id).Contains(usr.Id))
                .ToList();

            var result = adminsDetails
                .Select(adm => new
                {
                    adm.Id,
                    adm.Email,
                    adm.FirstName,
                    adm.LastName,
                    RoleName = admins.First(admin => admin.Id == adm.Id).Role.Name
                });

            var response = new GetAllCompanyAdminsResponse()
            {
                Pagination = new PaginationResponse()
                {
                    TotalCount = totalCount
                }
            };
            response.CompanyAdmins.AddRange(result.Select(adm => new UserData()
            {
                Id = adm.Id.ToString(),
                Email = adm.Email,
                FirstName = adm.FirstName,
                LastName = adm.LastName,
                RoleName = adm.RoleName
            }));

            return response;
        }

        public async Task<bool> CheckLinkIsValidAsync(Guid userId, Guid linkId)
        {
            Link link = await _adminContext.Links.FirstOrDefaultAsync(s => s.UserId == userId && s.Id == linkId);

            if (link == null)
            {
                return false;
            }

            if (link.IsEpxired || link.DateExpires < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }

        public async Task<GetUserResponse> GetUserByEmailAsync(GetUserByEmailRequest request)
        {
            var user = await _adminContext.Users
                .Include(user => user.Role)
                    .ThenInclude(role => role.RoleFeatures)
                        .ThenInclude(roleFeature => roleFeature.Feature)
                .FirstAsync(user => user.IsActive && user.Email == request.Email);

            return await GetUserDetails(user);
        }

        public async Task<GetUserResponse> GetUserAsync(GetUserRequest request)
        {
            var user = await _adminContext.Users
                .Include(user => user.Role)
                    .ThenInclude(role => role.RoleFeatures)
                        .ThenInclude(roleFeature => roleFeature.Feature)
                .FirstAsync(user => user.IsActive && user.Id == Guid.Parse(request.UserId));

            return await GetUserDetails(user);
        }

        public async Task<GetAllCompanyUsersResponse> GetAllCompanyUsersAsync(GetAllCompanyUsersRequest request)
        {
            //Get users
            var users = await _adminContext.Users
                .Include(user => user.Role)
                    .ThenInclude(role => role.RoleFeatures)
                        .ThenInclude(roleFeature => roleFeature.Feature)
                .Where(user => user.IsActive && user.Role.CompanyId == request.CompanyId)
                .OrderBy(user => user.Email)
                .Skip(request.Pagination.Offset)
                .Take(request.Pagination.Limit)
                .ToListAsync();

            var totalCount = await _adminContext.Users
                .Where(user => user.IsActive && user.Role.CompanyId == request.CompanyId)
                .CountAsync();

            //Get users details
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var usersDetails = await clientContext.UserDetails
                .Where(details => users
                    .Select(user => user.Id)
                    .Contains(details.Id))
                .ToListAsync();

            //Create users models
            var usersModels = new List<ExtendedUserData>();

            foreach (var user in users)
            {
                var userDetails = usersDetails.First(userDetails => userDetails.Id == user.Id);

                var userModel = new ExtendedUserData()
                {
                    Id = user.Id.ToString(),
                    Email = user.Email,
                    IsActive = user.IsActive,
                    Role = new Protos.Role()
                    {
                        Id = user.Role.Id,
                        Description = user.Role.Description,
                        Name = user.Role.Name,
                        Features = 
                        {
                            user.Role.RoleFeatures.Select(feature => new Protos.Feature()
                            {
                                Id = feature.Feature.Id,
                                Description = feature.Feature.Description,
                                Name = feature.Feature.Name
                            })
                        }
                    },
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    Address = userDetails.Address,
                    Notes = userDetails.Notes,
                    PhoneNumber = userDetails.PhoneNumber
                };

                usersModels.Add(userModel);
            }

            var result = new GetAllCompanyUsersResponse()
            {
                Pagination = new PaginationResponse()
                {
                    TotalCount = totalCount
                }
            };
            result.Users.AddRange(usersModels);

            return result;
        }

        public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
        {
            //Create user
            var user = new User()
            {
                Created = DateTime.Now,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = new byte[0],
                PasswordSalt = new byte[0],
                RoleId = request.RoleId,
                IsActive = true
            };

            _adminContext.Users.Add(user);

            await _adminContext.SaveChangesAsync();

            //Create user in clientDb
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            await clientContext.UserDetails.AddAsync(new UserDetails
            {
                Id = user.Id,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Notes = request.Notes,
                Address = request.Address
            });
            await clientContext.SaveChangesAsync();

            await SendResetPasswordEmailAsync(user.Id, user.Email);

            return new CreateUserResponse()
            {
                UserId = user.Id.ToString()
            };
        }


        public async Task<string> SendForgotPasswordEmailAsync(string userEmail)
        {
            var userId = await _efContextFactory.GetAdminContext().Users.Where(s => s.Email == userEmail).Select(s => s.Id).FirstOrDefaultAsync();

            if(userId == Guid.Empty)
            {
                return "User with this email not found";
            }
            await SendResetPasswordEmailAsync(userId, userEmail);

            return String.Empty;
        }

        public async Task<Empty> UpdateUserAsync(UpdateUserRequest request)
        {
            //TODO: Make transaction here

            //Update user in admin DB
            User user = await _adminContext.Users
                .Include(user => user.Role)
                .FirstAsync(user => user.IsActive && user.Id == Guid.Parse(request.UserId));

            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.RoleId = request.RoleId;

            await _adminContext.SaveChangesAsync();

            //Update user in client DB
            var clientContext = await _efContextFactory.GetRequiredContext(user.Role.CompanyId.Value);

            var userDetails = await clientContext.UserDetails
                .FirstAsync(user => user.Id == Guid.Parse(request.UserId));

            userDetails.Email = request.Email;
            userDetails.FirstName = request.FirstName;
            userDetails.LastName = request.LastName;
            userDetails.PhoneNumber = request.PhoneNumber;
            userDetails.Notes = request.Notes;
            userDetails.Address = request.Address;

            await clientContext.SaveChangesAsync();

            return new Empty();
        }

        public async Task<Empty> DisableUserAsync(DisableUserRequest request)
        {
            var user = await _adminContext.Users
                .FirstAsync(user => user.Id == Guid.Parse(request.UserId));

            user.IsActive = false;

            await _adminContext.SaveChangesAsync();

            return new Empty();
        }

        #region Private routine

        private async Task SendResetPasswordEmailAsync(Guid userId, string email)
        {
            Link link = new Link()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                DateCreated = DateTime.UtcNow,
                DateExpires = DateTime.UtcNow.AddDays(5),
                IsEpxired = false
            };

            await _adminContext.Links.AddAsync(link);
            await _adminContext.SaveChangesAsync();

            string resetPasswordUrl = _emailLinkOptions.FrontEndHost + "/"
                + String.Format(_emailLinkOptions.FrontEndRoute, userId.ToString(), link.Id.ToString());

            string message = $"Hello, to set password please follow the <a href='{resetPasswordUrl}'>link</a>";

            await _notificatorGrpcClient.SendEmailAsync(email, "Set password", message);
        }

        private async Task<GetUserResponse> GetUserDetails(User user)
        {
            var response = new GetUserResponse()
            {
                User = new ExtendedUserData()
                {
                    Id = user.Id.ToString(),
                    IsActive = user.IsActive,
                    Email = user.Email,
                    Role = new Protos.Role()
                    {
                        Id = user.Role.Id,
                        Description = user.Role.Description,
                        Name = user.Role.Name,
                        Features =
                        {
                            user.Role.RoleFeatures.Select(roleFeature => new Protos.Feature()
                            {
                                Id = roleFeature.Feature.Id,
                                Description = roleFeature.Feature.Description,
                                Name = roleFeature.Feature.Name
                            })
                        }
                    },
                }
            };

            if (!user.Role.CompanyId.HasValue)
            {
                return response;
            }

            var clientContext = await _efContextFactory.GetRequiredContext(user.Role.CompanyId.Value);

            var userDetails = await clientContext.UserDetails
                .FirstAsync(details => details.Id == user.Id);

            response.User.Address = userDetails.Address;
            response.User.FirstName = userDetails.FirstName;
            response.User.LastName = userDetails.LastName;
            response.User.PhoneNumber = userDetails.PhoneNumber;
            response.User.Notes = userDetails.Notes;

            return response;
        }

        #endregion Private routine
    }
}
