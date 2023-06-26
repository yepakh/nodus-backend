using Microsoft.EntityFrameworkCore;
using Nodus.Database.Context;
using Nodus.Jamal.Service.Protos;
using Bill = Nodus.Database.Models.Customer.Bill;

namespace Nodus.Jamal.Service.Services
{
    public class StatisticsSingleTripService
    {
        private readonly EFContextFactory _efContextFactory;

        public StatisticsSingleTripService(EFContextFactory efContextFactory)
        {
            _efContextFactory = efContextFactory;
        }

        public async Task<GetTripStatisticsTotalBillsResponse> GetTripStatisticsTotalBillsAsync(GetTripStatisticsTotalBillsRequest request)
        {
            var clientContext = await GetClientContext(request.Filters);

            var billsRequest = GetFitleredBillsRequest(request.Filters, clientContext);

            double totalSum = await billsRequest.DefaultIfEmpty().SumAsync(s => s.Summary);
            int totalChecks = await billsRequest.CountAsync();

            var bills = await ApplyPaginationOnBillRequest(billsRequest, request.Pagination)
                .Select(s => new BillShortView()
                {
                    BillId = s.Id,
                    Name = s.Name,
                    Description = s.Desciption,
                    Category = s.BillCategory.Name,
                    Summary = s.Summary,
                    DateCreated = s.DateTimeCreated.ToString(),
                    CreatorName = s.Creator.FirstName + " " + s.Creator.LastName,
                })
                .ToListAsync();

            GetTripStatisticsTotalBillsResponse response = new GetTripStatisticsTotalBillsResponse();
            response.Pagination = new PaginationResponse() { TotalCount = totalChecks };
            response.TotalSpending = totalSum;
            response.Bills.AddRange(bills);

            return response;
        }

        public async Task<GetTripStatisticsByCategoryResponse> GetTripStatisticsByCategoryAsync(GetTripStatisticsRequest request)
        {
            var clientContext = await GetClientContext(request);

            var billsRequest = GetFitleredBillsRequest(request, clientContext);

            var groupedBills = await billsRequest
                .GroupBy(s => new { s.BillCategory.Id })
                .Select(s => new { s.First().BillCategory.Name, s.Key.Id, Sum = s.DefaultIfEmpty().Sum(p => p.Summary) })
                .ToListAsync();

            var totalSum = groupedBills.Sum(s => s.Sum);
            var reponseItems = groupedBills.Select(s => new ShortIntItemStatisticsMessage()
            {
                ItemId = s.Id,
                ItemName = s.Name,
                Value = s.Sum,
                PctOfTotal = totalSum > 0 ? Math.Round(s.Sum * 100 / totalSum, 2) : 0
            });

            GetTripStatisticsByCategoryResponse response = new GetTripStatisticsByCategoryResponse();
            response.Total = totalSum;
            response.Categories.AddRange(reponseItems);

            return response;
        }

        public async Task<GetTripStatisticsByUserResponse> GetTripStatisticsByUserAsync(GetTripStatisticsRequest request)
        {
            var clientContext = await GetClientContext(request);

            var billsRequest = GetFitleredBillsRequest(request, clientContext);

            var groupedBills = await billsRequest
                .GroupBy(s => new { s.Creator.Id })
                .Select(s => new
                {
                    Name = s.First().Creator.FirstName + " " + s.First().Creator.LastName,
                    s.Key.Id,
                    Sum = s.DefaultIfEmpty().Sum(p => p.Summary),
                    DailyExpensesSum = s.Where(s => s.BillCategory.IsIncludedInDailyAllowance).DefaultIfEmpty().Sum(p => p.Summary),
                })
                .ToListAsync();

            var userLimits = await clientContext.UserTrips
                .Include(s => s.Trip)
                .Where(s => s.TripId == request.TripId)
                .Select(s => new { s.UserId, Limit = s.Budget * EF.Functions.DateDiffDay(s.Trip.StartOfTrip, s.Trip.EndOfTrip) })
                .ToListAsync();

            var totalSum = groupedBills.Sum(s => s.Sum);
            var reponseItems = groupedBills.Select(s => new ShortUserStatisticsItemMessage()
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Sum = s.Sum,
                SumInDailyExpenses = s.DailyExpensesSum,
                PctOfTotal = totalSum > 0 ? Math.Round(s.Sum * 100 / totalSum, 2) : 0,
                Limit = userLimits.FirstOrDefault(l => l.UserId == s.Id)?.Limit ?? 0
            });

            GetTripStatisticsByUserResponse response = new GetTripStatisticsByUserResponse();
            response.Total = totalSum;
            response.Users.AddRange(reponseItems);

            return response;
        }

        private IQueryable<Bill> GetFitleredBillsRequest(GetTripStatisticsRequest request, ClientContext clientContext)
        {
            var billsRequest = clientContext.Bills
                .Include(s => s.BillCategory)
                .Include(s => s.Creator)
                .Where(s => s.TripId == request.TripId);

            if (request.UserIds.Any())
            {
                billsRequest = billsRequest.Where(s => request.UserIds.Select(g => new Guid(g)).Contains(s.CreatorId));
            }

            if (request.CategoryIds.Any())
            {
                billsRequest = billsRequest.Where(s => request.CategoryIds.Contains(s.BillCategoryId));
            }

            return billsRequest;
        }

        private async Task<ClientContext> GetClientContext(GetTripStatisticsRequest request)
        {
            return await _efContextFactory.GetContext(request.CompanyId);
        }

        private static IQueryable<Bill> ApplyPaginationOnBillRequest(IQueryable<Bill> billsRequest, PaginationRequest request)
        {
            return billsRequest
                .Skip(request.Offset)
                .Take(request.Limit);
        }
    }
}
