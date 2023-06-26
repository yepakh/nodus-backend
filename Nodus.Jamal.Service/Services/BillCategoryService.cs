using Microsoft.EntityFrameworkCore;
using Nodus.Database.Context;
using Nodus.Jamal.Service.Protos;

namespace Nodus.Jamal.Service.Services
{
    public class BillCategoryService
    {
        private readonly AdminContext _adminContext;
        private readonly EFContextFactory _efContextFactory;

        public BillCategoryService(AdminContext adminContext, EFContextFactory efContextFactory)
        {
            _adminContext = adminContext;
            _efContextFactory = efContextFactory;
        }

        public async Task<BillCategoryInfoResponse> GetBillCategoryInfoAsync(GetBillCategoryRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var groupedInfo = await clientContext.Bills
                .Include(s => s.Trip)
                .Where(s => s.BillCategoryId == request.BillCategoryId)
                .GroupBy(s => s.TripId)
                .Select(s => new BillUsageInTripInfo()
                {
                    TripId = s.Key,
                    TripName = s.First().Trip.Name,
                    BillsInCategory = s.Count(),
                    SumOfBillsInCategory = s.DefaultIfEmpty().Sum(s => s.Summary)
                })
                .ToListAsync();

            var totalBills = groupedInfo.Sum(s => s.BillsInCategory);
            var totalSpendings = groupedInfo.Sum(s => s.SumOfBillsInCategory);

            var response = new BillCategoryInfoResponse();

            response.TotalSpendings = totalSpendings;
            response.TotalBills = totalBills;
            response.Trips.AddRange(groupedInfo);

            return response;
        }

        public async Task<GetBillCategoryResponse> GetBillCategoryAsync(GetBillCategoryRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var category = await clientContext.BillCategories
                .FirstAsync(category => category.Id == request.BillCategoryId);

            return new GetBillCategoryResponse()
            {
                BillCategory = new BillCategory()
                {
                    Id = category.Id,
                    Description = category.Description,
                    Name = category.Name,
                    IsIncludedInDailyAllowance = category.IsIncludedInDailyAllowance
                }
            };
        }

        public async Task<GetBillCategoriesResponse> GetBillCategoriesAsync(GetBillCategoriesRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var totalCount = await clientContext.BillCategories.CountAsync();

            var categories = await clientContext.BillCategories
                .Select(category => new BillCategory()
                {
                    Description = category.Description,
                    Name = category.Name,
                    Id = category.Id,
                    IsIncludedInDailyAllowance = category.IsIncludedInDailyAllowance
                })
                .OrderBy(category => category.Name)
                .Skip(request.Pagination.Offset)
                .Take(request.Pagination.Limit)
                .ToListAsync();

            return new GetBillCategoriesResponse()
            {
                Pagination = new PaginationResponse()
                {
                    TotalCount = totalCount
                },
                BillCategories = { categories }
            };
        }
        
        public async Task<CreateBillCategoryResponse> CreateBillCategoryAsync(CreateBillCategoryRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var category = new Database.Models.Customer.BillCategory()
            {
                Name = request.Name,
                Description = request.Description,
                IsIncludedInDailyAllowance = request.IsIncludedInDailyAllowance
            };

            await clientContext.BillCategories.AddAsync(category);
            await clientContext.SaveChangesAsync();

            return new CreateBillCategoryResponse() { BillCategoryId = category.Id };
        }

        public async Task<Empty> UpdateBillCategoryAsync(UpdateBillCategoryRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var category = await clientContext.BillCategories
                .FirstAsync(category => category.Id == request.BillCategoryId);

            category.Name = request.Name;
            category.Description = request.Description;
            category.IsIncludedInDailyAllowance = request.IsIncludedInDailyAllowance;

            await clientContext.SaveChangesAsync();

            return new Empty();
        }

        public async Task<Empty> DeleteBillCategoryAsync(DeleteBillCategoryRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var category = await clientContext.BillCategories
                .FirstAsync(category => category.Id == request.BillCategoryId);

            clientContext.BillCategories.Remove(category);
            await clientContext.SaveChangesAsync();

            return new Empty();
        }
    }
}
