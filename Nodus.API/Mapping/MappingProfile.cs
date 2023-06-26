using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Nodus.API.Models.Bill;
using Nodus.API.Models.BillCategory;
using Nodus.API.Models.Common;
using Nodus.API.Models.Company;
using Nodus.API.Models.Feature;
using Nodus.API.Models.Role;
using Nodus.API.Models.SingleTripStatistics;
using Nodus.API.Models.TotalStatistics;
using Nodus.API.Models.Trip;
using Nodus.API.Models.User;
using Nodus.Database.Models.Customer;
using Nodus.Jamal.Service.Protos;
using Bill = Nodus.Jamal.Service.Protos.Bill;
using BillCategory = Nodus.Jamal.Service.Protos.BillCategory;
using Trip = Nodus.Jamal.Service.Protos.Trip;

namespace Nodus.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Timestamp, DateTime>()
                .ConvertUsing<TimestampToDateTimeConverter>();

            CreateMap<CreateCompanyRequestModel, CreateCompanyRequest>()
                .ForMember(dest => dest.ConnectionString, opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CreateCompanyResponse, CreateCompanyResponseModel>();

            CreateMap<int, DeleteCompanyRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src));

            CreateMap<int, GetCompanyByIdRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src));

            CreateMap<GetCompanyByIdResponse, CompanyViewModel>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Company.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Company.Name))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Company.Description))
                .ForMember(d => d.TotalAdminsCount, opt => opt.MapFrom(src => src.Company.TotalAdminsCount));

            CreateMap<AllCompaniesRequestModel, GetCompaniesRequest>();

            CreateMap<RequestPagination, Nodus.Jamal.Service.Protos.PaginationRequest>();

            CreateMap<CompanyData, CompanyViewModel>();

            CreateMap<AllCompanyAdminsRequestModel, GetAllCompanyAdminsRequest>();

            CreateMap<UserData, CompanyAdminViewModel>();

            CreateMap<Feature, FeatureViewModel>();

            CreateMap<int, DeleteRoleRequest>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src));

            CreateMap<UpdateRoleRequestModel, UpdateRoleRequest>();

            CreateMap<(CreateRoleRequestModel Request, int CompanyId), CreateRoleRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Request.Name))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Request.Description))
                .ForMember(d => d.AvaliableFeaturesIds, opt => opt.MapFrom(src => src.Request.AvaliableFeaturesIds));

            CreateMap<(AllCompanyRolesRequestModel Request, int CompanyId), GetRolesRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.Pagination, opt => opt.MapFrom(src => src.Request.Pagination));

            CreateMap<Role, RoleViewModel>();

            CreateMap<int, GetRoleRequest>()
                .ForMember(d => d.RoleId, opt => opt.MapFrom(src => src));

            CreateMap<string, GetUserRequest>()
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src));

            CreateMap<(AllCompanyUsersRequestModel request, int CompanyId), GetAllCompanyUsersRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.Pagination, opt => opt.MapFrom(src => src.request.Pagination));

            CreateMap<(CreateUserRequestModel Request, int CompanyId), CreateUserRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.Address, opt => opt.MapFrom(src => src.Request.Address))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Request.Email))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.Request.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.Request.LastName))
                .ForMember(d => d.Notes, opt => opt.MapFrom(src => src.Request.Notes))
                .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(src => src.Request.PhoneNumber))
                .ForMember(d => d.RoleId, opt => opt.MapFrom(src => src.Request.RoleId));

            CreateMap<string, DisableUserRequest>()
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src));

            CreateMap<ExtendedUserData, UserViewModel>();

            CreateMap<UpdateUserRequestModel, UpdateUserRequest>();

            CreateMap<(int TripId, int CompanyId), GetTripRequest>()
                .ForMember(d => d.TripId, opt => opt.MapFrom(src => src.TripId))
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId));

            CreateMap<Trip, TripViewModel>()
                .ForMember(d => d.Budget, opt => opt.Condition(src => src.Budget >= 0))
                .ForMember(d => d.TripStatus, opt => opt.MapFrom(src => (TripStatusEnum)System.Enum.Parse(typeof(TripStatusEnum), src.TripStatus)));

            CreateMap<(Guid UserId, int CompanyId, AllUserTripsRequestModel Request), GetUserTripsRequest>()
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.Pagination, opt => opt.MapFrom(src => src.Request.Pagination))
                .ForMember(d => d.TripRole, opt => opt.MapFrom(src => src.Request.TripRole))
                .ForMember(d => d.TripStatus, opt => opt.MapFrom(src => src.Request.TripStatus));

            CreateMap<(int CompanyId, GetTripUsersRequestModel Request), GetTripUsersRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.TripId, opt => opt.MapFrom(src => src.Request.TripId))
                .ForMember(d => d.Pagination, opt => opt.MapFrom(src => src.Request.Pagination));

            CreateMap<TripUser, TripUserViewModel>();

            CreateMap<TripUserRequestModel, TripUserRequest>();

            CreateMap<(CreateTripRequestModel Request, int CompanyId, Guid CreatorId), CreateTripRequest>()
                .ForMember(d => d.CreatedById, opt => opt.MapFrom(src => src.CreatorId))
                .ForMember(d => d.DateTimeEnd, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.Request.DateTimeEnd.ToUniversalTime())))
                .ForMember(d => d.DateTimeStart, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.Request.DateTimeStart.ToUniversalTime())))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Request.Description))
                .ForMember(d => d.Budget, opt => opt.MapFrom(src => src.Request.Budget))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Request.Name))
                .ForMember(d => d.Users, opt => opt.MapFrom(src => src.Request.Users))
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId));

            CreateMap<(UpdateTripDataRequestModel Request, int CompanyId), UpdateTripDataRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.DateTimeEnd, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.Request.DateTimeEnd.ToUniversalTime())))
                .ForMember(d => d.DateTimeStart, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.Request.DateTimeStart.ToUniversalTime())))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Request.Description))
                .ForMember(d => d.Budget, opt => opt.MapFrom(src => src.Request.Budget))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Request.Name))
                .ForMember(d => d.TripId, opt => opt.MapFrom(src => src.Request.TripId));

            CreateMap<(int TripId, int CompanyId), DeleteTripRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.TripId, opt => opt.MapFrom(src => src.TripId));

            CreateMap<(ChangeTripStatusRequestModel request, int CompanyId), ChangeTripStatusRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.StatusId, opt => opt.MapFrom(src => (int)src.request.NewStatus))
                .ForMember(d => d.TripId, opt => opt.MapFrom(src => src.request.TripId));

            CreateMap<(AddUserToTripRequestModel request, int CompanyId), UpdateTripUsersRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.Users, opt => opt.MapFrom(src => src.request.Users))
                .ForMember(d => d.TripId, opt => opt.MapFrom(src => src.request.TripId));

            CreateMap<(int CategoryId, int CompanyId), GetBillCategoryRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.BillCategoryId, opt => opt.MapFrom(src => src.CategoryId));

            CreateMap<BillCategory, BillCategoryViewModel>();

            CreateMap<(RequestPagination Pagination, int CompanyId), GetBillCategoriesRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.Pagination, opt => opt.MapFrom(src => src.Pagination));

            CreateMap<(CreateBillCategoryRequestModel Request, int CompanyId), CreateBillCategoryRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Request.Name))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Request.Description))
                .ForMember(d => d.IsIncludedInDailyAllowance, opt => opt.MapFrom(src => src.Request.IsIncludedInDailyAllowance));

            CreateMap<(UpdateBillCategoryRequestModel Request, int CompanyId), UpdateBillCategoryRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Request.Name))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Request.Description))
                .ForMember(d => d.BillCategoryId, opt => opt.MapFrom(src => src.Request.Id))
                .ForMember(d => d.IsIncludedInDailyAllowance, opt => opt.MapFrom(src => src.Request.IsIncludedInDailyAllowance));

            CreateMap<(int CategoryId, int CompanyId), DeleteBillCategoryRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.BillCategoryId, opt => opt.MapFrom(src => src.CategoryId));

            CreateMap<GetTripStatisticsRequestModel, GetTripStatisticsRequest>()
                .ForMember(d => d.UserIds, opt => opt.MapFrom(src => src.UserIds.Select(s => s.ToString())))
                .ForMember(d => d.CompanyId, opt => opt.Ignore());

            CreateMap<GetTripStatisticsTotalBillsRequestModel, GetTripStatisticsTotalBillsRequest>();

            CreateMap<TotalStatisticsFilterRequestModel, TotalStatisticsFilterRequest>()
                .ForMember(d => d.UserIds, opt => opt.MapFrom(src => src.UserIds.Select(s => s.ToString())))
                .ForMember(d => d.StartDate, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.StartDate.ToUniversalTime())))
                .ForMember(d => d.EndDate, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.EndDate.ToUniversalTime())))
                .ForMember(d => d.CompanyId, opt => opt.Ignore());
            CreateMap<(CreateBillRequestModel Request, int CompanyId, Guid CreatorId), CreateBillRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.CategoryId, opt => opt.MapFrom(src => src.Request.CategoryId))
                .ForMember(d => d.CreatedById, opt => opt.MapFrom(src => src.CreatorId.ToString()))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Request.Description))
                .ForMember(d => d.DocumentsUrls, opt => opt.MapFrom(src => src.Request.DocumentsUrls))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Request.Name))
                .ForMember(d => d.Sumary, opt => opt.MapFrom(src => src.Request.Sumary))
                .ForMember(d => d.TripId, opt => opt.MapFrom(src => src.Request.TripId));

            CreateMap<(UpdateBillRequestModel Request, int CompanyId, Guid UpdatedById), UpdateBillRequest>()
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(d => d.CategoryId, opt => opt.MapFrom(src => src.Request.CategoryId))
                .ForMember(d => d.UpdatedById, opt => opt.MapFrom(src => src.UpdatedById.ToString()))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Request.Description))
                .ForMember(d => d.DocumentsUrls, opt => opt.MapFrom(src => src.Request.DocumentsUrls))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Request.Name))
                .ForMember(d => d.Sumary, opt => opt.MapFrom(src => src.Request.Sumary))
                .ForMember(d => d.Sumary, opt => opt.MapFrom(src => src.Request.Sumary))
                .ForMember(d => d.TripId, opt => opt.MapFrom(src => src.Request.TripId))
                .ForMember(d => d.BillId, opt => opt.MapFrom(src => src.Request.BillId));

            CreateMap<Bill, BillViewModel>()
                .ForMember(d => d.Status, opt => opt.MapFrom(src => (int)src.Status))
                .ForMember(d => d.DocumentsIds, opt => opt.MapFrom(src => src.DocumentsIds.ToList()))
                .ForMember(d => d.DocumentsUrls, opt => opt.MapFrom(src => src.DocumentsUrls.ToList()));
        }
    }
}
