using Microsoft.EntityFrameworkCore;
using Nodus.Database.Context;
using Nodus.Jamal.Service.Protos;
using Bill = Nodus.Database.Models.Customer.Bill;

namespace Nodus.Jamal.Service.Services
{
    public class TotalStatisticsService
    {
        private readonly EFContextFactory _efContextFactory;

        public TotalStatisticsService(EFContextFactory efContextFactory)
        {
            _efContextFactory = efContextFactory;
        }

        public async Task<TotalStatisticsInfoResponse> GetTotalStatisticsInfoAsync(TotalStatisticsFilterRequest request)
        {
            ClientContext clientContext = await GetClientContext(request);
            var billsQuery = GetFitleredBillsRequest(request, clientContext);

            double totalExpenses = await billsQuery.DefaultIfEmpty().SumAsync(s => s.Summary);

            double avgPerTrip = await billsQuery
                .GroupBy(s => s.TripId)
                .Select(s => new { s.Key, Avereage = s.DefaultIfEmpty().Sum(p => p.Summary) })
                .DefaultIfEmpty()
                .AverageAsync(s => s.Avereage);

            double avgPerBill = await billsQuery.DefaultIfEmpty().AverageAsync(s => s.Summary);
            int totalCount = await billsQuery.CountAsync();

            TotalStatisticsInfoResponse response = new TotalStatisticsInfoResponse();
            response.TotalBills = totalCount;
            response.AvgPerTrip = avgPerTrip;
            response.AvgPerBill = avgPerBill;
            response.TotalExpenses = totalExpenses;

            return response;
        }

        public async Task<GetTripStatisticsByCategoryResponse> GetTotalStatisticsByCategoryAsync(TotalStatisticsFilterRequest request)
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

        public async Task<GetTripStatisticsByUserResponse> GetTotalStatisticsByUserAsync(TotalStatisticsFilterRequest request)
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

            var totalSum = groupedBills.Sum(s => s.Sum);
            var reponseItems = groupedBills.Select(s => new ShortUserStatisticsItemMessage()
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Sum = s.Sum,
                SumInDailyExpenses = s.DailyExpensesSum,
                PctOfTotal = totalSum > 0 ? Math.Round(s.Sum * 100 / totalSum, 2) : 0,
                Limit = 0
            });

            GetTripStatisticsByUserResponse response = new GetTripStatisticsByUserResponse();
            response.Total = totalSum;
            response.Users.AddRange(reponseItems);

            return response;
        }

        public async Task<GetTotalExpensesByTripsResponse> GetTotalStatisticsByTripsAsync(TotalStatisticsFilterRequest request)
        {
            var clientContext = await GetClientContext(request);

            var billsRequest = GetFitleredBillsRequest(request, clientContext);

            var groupedBills = await billsRequest
                .GroupBy(s => new { s.Trip.Id })
                .Select(s => new { s.First().Trip.Name, s.Key.Id, Sum = s.DefaultIfEmpty().Sum(p => p.Summary) })
                .ToListAsync();

            var totalSum = groupedBills.Sum(s => s.Sum);
            var reponseItems = groupedBills.Select(s => new ShortIntItemStatisticsMessage()
            {
                ItemId = s.Id,
                ItemName = s.Name,
                Value = s.Sum,
                PctOfTotal = totalSum > 0 ? Math.Round(s.Sum * 100 / totalSum, 2) : 0
            });

            GetTotalExpensesByTripsResponse response = new GetTotalExpensesByTripsResponse();
            response.Total = totalSum;
            response.Trips.AddRange(reponseItems);

            return response;
        }

        private IQueryable<Bill> GetFitleredBillsRequest(TotalStatisticsFilterRequest request, ClientContext clientContext)
        {
            var billsRequest = clientContext.Bills
                .Include(s => s.Trip)
                .Include(s => s.BillCategory)
                .Include(s => s.Creator)
                .Where(s => s.DateTimeCreated.Date >= request.StartDate.ToDateTime() && s.DateTimeCreated.Date <= request.EndDate.ToDateTime());

            if (request.TripIds.Any())
            {
                billsRequest = billsRequest.Where(s => request.TripIds.Contains(s.TripId));
            }

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

        private async Task<ClientContext> GetClientContext(TotalStatisticsFilterRequest request)
        {
            return await _efContextFactory.GetContext(request.CompanyId);
        }
    }
}
