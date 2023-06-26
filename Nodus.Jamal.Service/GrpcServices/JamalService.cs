using Grpc.Core;
using Nodus.Jamal.Service.Protos;
using Nodus.Jamal.Service.Services;

namespace Nodus.Jamal.Service.GrpcServices
{
    public class JamalService : Jam.JamBase
    {
        private readonly CompanyService _companyService;
        private readonly UserService _userService;
        private readonly RoleService _rolesService;
        private readonly FeatureService _featureService;
        private readonly TripService _tripService;
        private readonly BillCategoryService _billCategoryService;
        private readonly FileService _fileService;
        private readonly StatisticsSingleTripService _statisticsSingleTripService;
        private readonly TotalStatisticsService _totalStatisticsService;
        private readonly BillService _billService;

        public JamalService(
            CompanyService companyService,
            UserService userService,
            RoleService rolesService,
            FeatureService featureService,
            TripService tripService,
            BillCategoryService billCategoryService,
            FileService fileService,
            StatisticsSingleTripService statisticsSingleTripService,
            TotalStatisticsService totalStatisticsService,
            BillService billService)
        {
            _companyService = companyService;
            _userService = userService;
            _rolesService = rolesService;
            _featureService = featureService;
            _tripService = tripService;
            _billCategoryService = billCategoryService;
            _fileService = fileService;
            _statisticsSingleTripService = statisticsSingleTripService;
            _totalStatisticsService = totalStatisticsService;
            _billService = billService;
        }

        #region Company

        public override async Task<GetCompaniesResponse> GetCompanies(GetCompaniesRequest request, ServerCallContext context)
        {
            return await _companyService.GetCompaniesAsync(request);
        }

        public override async Task<GetCompanyByIdResponse> GetCompanyById(GetCompanyByIdRequest request, ServerCallContext context)
        {
            return new GetCompanyByIdResponse()
            {
                Company = await _companyService.GetCompanyByIdAsync(request.CompanyId)
            };
        }

        public override async Task<CreateCompanyResponse> CreateCompany(CreateCompanyRequest request, ServerCallContext context)
        {
            return new CreateCompanyResponse()
            {
                CompanyId = await _companyService.CreateCompanyAsync(request)
            };
        }

        public override async Task<Empty> DeleteCompany(DeleteCompanyRequest request, ServerCallContext context)
        {
            await _companyService.DeleteCompanyByIdAsync(request.CompanyId);

            return new Empty();
        }

        #endregion Company

        #region User

        public override async Task<GetAllCompanyAdminsResponse> GetAllCompanyAdmins(GetAllCompanyAdminsRequest request, ServerCallContext context)
        {
            return await _userService.GetAllCompanyAdminsAsync(request);
        }

        public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
        {
            return await _userService.GetUserAsync(request);
        }

        public override async Task<GetUserResponse> GetUserByEmail(GetUserByEmailRequest request, ServerCallContext context)
        {
            return await _userService.GetUserByEmailAsync(request);
        }

        public override async Task<GetAllCompanyUsersResponse> GetAllCompanyUsers(GetAllCompanyUsersRequest request, ServerCallContext context)
        {
            return await _userService.GetAllCompanyUsersAsync(request);
        }

        public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            return await _userService.CreateUserAsync(request);
        }
        
        public override async Task<Empty> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            return await _userService.UpdateUserAsync(request);
        }

        public override async Task<Empty> DisableUser(DisableUserRequest request, ServerCallContext context)
        {
            return await _userService.DisableUserAsync(request);
        }

        public override async Task<IsEmailSentResponse> SendForgotPasswordEmail(GetUserByEmailRequest request, ServerCallContext context)
        {
            return new IsEmailSentResponse()
            {
                Response = await _userService.SendForgotPasswordEmailAsync(request.Email)
            };
        }

        #endregion User

        #region Roles

        public override async Task<GetRoleResponse> GetRole(GetRoleRequest request, ServerCallContext context)
        {
            return await _rolesService.GetRoleAsync(request);
        }

        public override async Task<GetRolesResponse> GetRoles(GetRolesRequest request, ServerCallContext context)
        {
            return await _rolesService.GetRolesAsync(request);
        }

        public override async Task<CreateRoleResponse> CreateRole(CreateRoleRequest request, ServerCallContext context)
        {
            return await _rolesService.CreateRoleAsync(request);
        }

        public override async Task<Empty> UpdateRole(UpdateRoleRequest request, ServerCallContext context)
        {
            return await _rolesService.UpdateRoleAsync(request);
        }

        public override async Task<Empty> DeleteRole(DeleteRoleRequest request, ServerCallContext context)
        {
            return await _rolesService.DeleteRoleAsync(request);
        }

        #endregion Roles

        #region Feature

        public override async Task<GetFeaturesResponse> GetFeatures(Empty request, ServerCallContext context)
        {
            return await _featureService.GetFeaturesAsync();
        }

        #endregion Feature

        #region Trip

        public override async Task<GetTripResponse> GetTrip(GetTripRequest request, ServerCallContext context)
        {
            return await _tripService.GetTripByIdAsync(request);
        }

        public override async Task<GetUserTripsResponse> GetUserTrips(GetUserTripsRequest request, ServerCallContext context)
        {
            return await _tripService.GetUserTripsAsync(request);
        }

        public override async Task<GetTripUsersResponse> GetTripUsers(GetTripUsersRequest request, ServerCallContext context)
        {
            return await _tripService.GetTripUsersAsync(request);
        }

        public override async Task<CreateTripResponse> CreateTrip (CreateTripRequest request, ServerCallContext context)
        {
            return await _tripService.CreateTripAsync(request);
        }

        public override async Task<Empty> UpdateTripData(UpdateTripDataRequest request, ServerCallContext context)
        {
            return await _tripService.UpdateTripDataAsync(request);
        }
        
        public override async Task<Empty> DeleteTrip(DeleteTripRequest request, ServerCallContext context)
        {
            return await _tripService.DeleteTripAsync(request);
        }

        public override async Task<Empty> ChangeTripStatus(ChangeTripStatusRequest request, ServerCallContext context)
        {
            return await _tripService.ChangeTripStatusAsync(request);
        }

        public override async Task<Empty> UpdateTripUsers(UpdateTripUsersRequest request, ServerCallContext context)
        {
            return await _tripService.UpdateTripUsersAsync(request);
        }

        #endregion Trip

        #region BillCategory

        public override async Task<GetBillCategoryResponse> GetBillCategory(GetBillCategoryRequest request, ServerCallContext context)
        {
            return await _billCategoryService.GetBillCategoryAsync(request);
        }

        public override async Task<GetBillCategoriesResponse> GetBillCategories(GetBillCategoriesRequest request, ServerCallContext context)
        {
            return await _billCategoryService.GetBillCategoriesAsync(request);
        }

        public override async Task<CreateBillCategoryResponse> CreateBillCategory(CreateBillCategoryRequest request, ServerCallContext context)
        {
            return await _billCategoryService.CreateBillCategoryAsync(request);
        }

        public override async Task<Empty> UpdateBillCategory(UpdateBillCategoryRequest request, ServerCallContext context)
        {
            return await _billCategoryService.UpdateBillCategoryAsync(request);
        }

        public override async Task<Empty> DeleteBillCategory(DeleteBillCategoryRequest request, ServerCallContext context)
        {
            return await _billCategoryService.DeleteBillCategoryAsync(request);
        }

        public override async Task<BillCategoryInfoResponse> GetBillCategoryInfo(GetBillCategoryRequest request, ServerCallContext context)
        {
            return await _billCategoryService.GetBillCategoryInfoAsync(request);
        }

        #endregion BillCategory

        #region File

        public override async Task<GetFilePathResponse> GetFilePath(GetFilePathRequest request, ServerCallContext context)
        {
            return new GetFilePathResponse()
            {
                Path = _fileService.GenerateFilePath(request.Extension)
            };
        }

        #endregion

        #region Single trip statistics

        public override async Task<GetTripStatisticsTotalBillsResponse> GetTripStatisticsTotalBills(GetTripStatisticsTotalBillsRequest request, ServerCallContext context)
        {
            return await _statisticsSingleTripService.GetTripStatisticsTotalBillsAsync(request);
        }

        public override async Task<GetTripStatisticsByCategoryResponse> GetTripStatisticsByCategory(GetTripStatisticsRequest request, ServerCallContext context)
        {
            return await _statisticsSingleTripService.GetTripStatisticsByCategoryAsync(request);
        }

        public override async Task<GetTripStatisticsByUserResponse> GetTripStatisticsByUser(GetTripStatisticsRequest request, ServerCallContext context)
        {
            return await _statisticsSingleTripService.GetTripStatisticsByUserAsync(request);
        }

        #endregion Single trip statistics

        #region Total statistics

        public override async Task<TotalStatisticsInfoResponse> GetTotalStatisticsInfo(TotalStatisticsFilterRequest request, ServerCallContext context)
        {
            return await _totalStatisticsService.GetTotalStatisticsInfoAsync(request);
        }

        public override async Task<GetTotalExpensesByTripsResponse> GetTotalExpensesByTrips(TotalStatisticsFilterRequest request, ServerCallContext context)
        {
            return await _totalStatisticsService.GetTotalStatisticsByTripsAsync(request);
        }

        public override async Task<GetTripStatisticsByCategoryResponse> GetTotalExpensesByCategories(TotalStatisticsFilterRequest request, ServerCallContext context)
        {
            return await _totalStatisticsService.GetTotalStatisticsByCategoryAsync(request);
        }

        public override async Task<GetTripStatisticsByUserResponse> GetTotalExpensesByUsers(TotalStatisticsFilterRequest request, ServerCallContext context)
        {
            return await _totalStatisticsService.GetTotalStatisticsByUserAsync(request);
        }

        #endregion Total statistics

        #region Bill

        public override async Task<GetBillResponse> GetBill(GetBillRequest request, ServerCallContext context)
        {
            return await _billService.GetBillAsync(request);
        }

        public override async Task<GetUserBillsResponse> GetUserBills(GetUserBillsRequest request, ServerCallContext context)
        {
            return await _billService.GetUserBillsAsync(request);
        }

        public override async Task<GetTripBillsResponse> GetTripBills(GetTripBillsRequest request, ServerCallContext context)
        {
            return await _billService.GetTripBillsAsync(request);
        }

        public override async Task<CreateBillResponse> CreateBill(CreateBillRequest request, ServerCallContext context)
        {
            return await _billService.CreateBillAsync(request);
        }

        public override async Task<Empty> UpdateBill(UpdateBillRequest request, ServerCallContext context)
        {
            return await _billService.UpdateBillAsync(request);
        }

        public override async Task<Empty> DeleteBill(DeleteBillRequest request, ServerCallContext context)
        {
            return await _billService.DeleteBillAsync(request);
        }

        #endregion Bill
    }
}
