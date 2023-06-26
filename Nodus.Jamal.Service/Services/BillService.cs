using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Nodus.Database.Context;
using Nodus.Database.Models.Customer;
using Nodus.gRPC.ExceptionHandler.CustomExceptions;
using Nodus.Jamal.Service.Protos;
using Empty = Nodus.Jamal.Service.Protos.Empty;

namespace Nodus.Jamal.Service.Services
{
    public class BillService
    {
        private readonly EFContextFactory _efContextFactory;

        public BillService(EFContextFactory efContextFactory)
        {
            _efContextFactory = efContextFactory;
        }

        public async Task<GetBillResponse> GetBillAsync(GetBillRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var bill = await clientContext.Bills
                .Include(bill => bill.Documents)
                .Include(s => s.Creator)
                .Include(s => s.Editor)
                .Include(s => s.BillCategory)
                .FirstAsync(bill => bill.Id == request.BillId);

            return new GetBillResponse()
            {
                Bill = new Protos.Bill()
                {
                    Id = bill.Id,
                    CategoryId = bill.BillCategoryId,
                    CategoryName = bill.BillCategory.Name,
                    CreatedById = bill.CreatorId.ToString(),
                    CreatedByName = bill.Creator.FirstName + " " + bill.Creator.LastName,
                    DateTimeCreated = Timestamp.FromDateTime(bill.DateTimeCreated.ToUniversalTime()),
                    DateTimeEdited = Timestamp.FromDateTime(bill.DateTimeEdited.ToUniversalTime()),
                    Description = bill.Desciption,
                    DocumentsIds = { bill.Documents.Where(s => s.IsActive).Select(doc => doc.Id.ToString()).ToList() },
                    DocumentsUrls = { bill.Documents.Where(s => s.IsActive).Select(doc => doc.Link).ToList() },
                    EditedById = bill.EditorId.ToString(),
                    EditedByName = bill.Editor.FirstName + " " + bill.Editor.LastName,
                    Name = bill.Name,
                    Status = (Protos.BillStatus)bill.StatusId,
                    Sumary = bill.Summary,
                    TripId = bill.TripId,
                }
            };
        }

        public async Task<GetUserBillsResponse> GetUserBillsAsync(GetUserBillsRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var query = clientContext.Bills
                .Where(bill =>
                    bill.IsActive &&
                    bill.TripId == request.TripId &&
                    (bill.CreatorId == Guid.Parse(request.UserId) ||
                    bill.EditorId == Guid.Parse(request.UserId)));

            var totalCount = await query.CountAsync();

            var bills = await query
                .Include(bill => bill.Documents)
                .Include(s => s.Creator)
                .Include(s => s.Editor)
                .Include(s => s.BillCategory)
                .OrderByDescending(bill => bill.DateTimeEdited)
                .Skip(request.Pagination.Offset)
                .Take(request.Pagination.Limit)
                .ToListAsync();

            var billsModels = new List<Protos.Bill>();

            foreach (var bill in bills)
            {
                var model = new Protos.Bill()
                {
                    Id = bill.Id,
                    CategoryId = bill.BillCategoryId,
                    CategoryName = bill.BillCategory.Name,
                    CreatedById = bill.CreatorId.ToString(),
                    CreatedByName = bill.Creator.FirstName + " " + bill.Creator.LastName,
                    DateTimeCreated = Timestamp.FromDateTime(bill.DateTimeCreated.ToUniversalTime()),
                    DateTimeEdited = Timestamp.FromDateTime(bill.DateTimeEdited.ToUniversalTime()),
                    Description = bill.Desciption,
                    DocumentsIds = { bill.Documents.Select(doc => doc.Id.ToString()).ToList() },
                    DocumentsUrls = { bill.Documents.Select(doc => doc.Link).ToList() },
                    EditedById = bill.EditorId.ToString(),
                    EditedByName = bill.Editor.FirstName + " " + bill.Editor.LastName,
                    Name = bill.Name,
                    Status = (Protos.BillStatus)bill.StatusId,
                    Sumary = bill.Summary,
                    TripId = bill.TripId,
                };

                billsModels.Add(model);
            }

            return new GetUserBillsResponse()
            {
                Bills = { billsModels },
                Pagination = new PaginationResponse()
                {
                    TotalCount = totalCount
                }
            };
        }

        public async Task<GetTripBillsResponse> GetTripBillsAsync(GetTripBillsRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var query = clientContext.Bills
                .Where(bill =>
                    bill.IsActive &&
                    bill.TripId == request.TripId);

            var totalCount = await query.CountAsync();

            var bills = await query
                .Include(bill => bill.Documents)
                .Include(s => s.Creator)
                .Include(s => s.Editor)
                .Include(s => s.BillCategory)
                .OrderByDescending(bill => bill.DateTimeEdited)
                .Skip(request.Pagination.Offset)
                .Take(request.Pagination.Limit)
                .ToListAsync();

            var billsModels = new List<Protos.Bill>();

            foreach (var bill in bills)
            {
                var model = new Protos.Bill()
                {
                    Id = bill.Id,
                    CategoryId = bill.BillCategoryId,
                    CategoryName = bill.BillCategory.Name,
                    CreatedById = bill.CreatorId.ToString(),
                    CreatedByName = bill.Creator.FirstName + " " + bill.Creator.LastName,
                    DateTimeCreated = Timestamp.FromDateTime(bill.DateTimeCreated.ToUniversalTime()),
                    DateTimeEdited = Timestamp.FromDateTime(bill.DateTimeEdited.ToUniversalTime()),
                    Description = bill.Desciption,
                    DocumentsIds = { bill.Documents.Select(doc => doc.Id.ToString()).ToList() },
                    DocumentsUrls = { bill.Documents.Select(doc => doc.Link).ToList() },
                    EditedById = bill.EditorId.ToString(),
                    EditedByName = bill.Editor.FirstName + " " + bill.Editor.LastName,
                    Name = bill.Name,
                    Status = (Protos.BillStatus)bill.StatusId,
                    Sumary = bill.Summary,
                    TripId = bill.TripId,
                };

                billsModels.Add(model);
            }

            return new GetTripBillsResponse()
            {
                Bills = { billsModels },
                Pagination = new PaginationResponse()
                {
                    TotalCount = totalCount
                }
            };
        }

        public async Task<CreateBillResponse> CreateBillAsync(CreateBillRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var bill = new Database.Models.Customer.Bill()
            {
                Name = request.Name,
                TripId = request.TripId,
                BillCategoryId = request.CategoryId,
                CreatorId = Guid.Parse(request.CreatedById),
                DateTimeCreated = DateTime.UtcNow,
                DateTimeEdited = DateTime.UtcNow,
                Desciption = request.Description,
                EditorId = Guid.Parse(request.CreatedById),
                StatusId = (int)TripStatusEnum.New,
                Summary = request.Sumary,
                IsActive = true
            };
            await clientContext.Bills.AddAsync(bill);

            await CreateDocuments(clientContext, request.DocumentsUrls, bill);

            await clientContext.SaveChangesAsync();

            return new CreateBillResponse() { BillId = bill.Id };
        }

        public async Task<Empty> UpdateBillAsync(UpdateBillRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var bill = await clientContext.Bills
                .Include(bill => bill.Documents)
                .FirstAsync(bill => bill.Id == request.BillId);

            //create historical bill
            var historicalBill = new HistoricalBill()
            {
                BillId = bill.Id,
                DateTimeEdit = DateTime.UtcNow,
                EditorId = Guid.Parse(request.UpdatedById),
                Desciption = bill.Desciption,
                Name = bill.Name,
                StatusId = bill.StatusId,
                Summary = bill.Summary,
                Documents = bill.Documents
            };

            await clientContext.HistoricalBills.AddAsync(historicalBill);

            //update with documents
            bill.Name = request.Name;
            bill.Summary = request.Sumary;
            bill.BillCategoryId = request.CategoryId;
            bill.Desciption = request.Description;
            bill.EditorId = Guid.Parse(request.UpdatedById);
            bill.TripId = request.TripId;
            bill.DateTimeEdited = DateTime.UtcNow;

            var documentsUrls = await clientContext.Documents
                .Where(doc => doc.BillId == bill.Id)
                .Select(doc => doc.Link)
                .ToListAsync();

            if (!request.DocumentsUrls.OrderBy(url => url).SequenceEqual(documentsUrls.OrderBy(url => url)))
            {
                //deactivate unused
                var unusedUrls = documentsUrls.Except(request.DocumentsUrls);

                var docsToDeactivate = clientContext.Documents.Where(doc => unusedUrls.Contains(doc.Link) && doc.BillId == bill.Id);
                foreach (var doc in docsToDeactivate)
                {
                    doc.IsActive = false;
                    doc.DeactivatorId = Guid.Parse(request.UpdatedById);
                }

                //create new
                var newUrls = request.DocumentsUrls.Except(documentsUrls);
                await CreateDocuments(clientContext, newUrls, bill);

                //ensure that existing are active
                var existingUrls = request.DocumentsUrls.Intersect(documentsUrls);
                var existingDocs = clientContext.Documents.Where(doc => existingUrls.Contains(doc.Link) && doc.BillId == bill.Id);
                foreach (var doc in existingDocs)
                {
                    doc.IsActive = true;
                }
            }

            await clientContext.SaveChangesAsync();

            return new Empty();
        }

        public async Task<Empty> DeleteBillAsync(DeleteBillRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var billToRemove = await clientContext.Bills.FirstAsync(bill => bill.Id == request.BillId);
            billToRemove.IsActive = false;

            await clientContext.SaveChangesAsync();

            return new Empty();
        }


        #region Private routine

        private async Task CreateDocuments(ClientContext clientContext, IEnumerable<string> urls, Database.Models.Customer.Bill bill)
        {
            var newDocIds = urls.Select(url => ExtractDocumentId(url)).ToList();
            var existingDocIds = await clientContext.Documents.Select(doc => doc.Id).ToListAsync();

            if (existingDocIds.Intersect(newDocIds).Any())
            {
                throw new BadRequestException("This document already exists in the system and can not be added.");
            }

            var documents = new List<Document>();

            foreach (var url in urls)
            {
                var document = new Document()
                {
                    Id = ExtractDocumentId(url),
                    IsActive = true,
                    Bill = bill,
                    CreatorId = bill.CreatorId,
                    DateTimeCreated = bill.DateTimeCreated,
                    Link = url
                };

                documents.Add(document);
            }

            await clientContext.Documents.AddRangeAsync(documents);
        }

        private Guid ExtractDocumentId(string url)
        {
            int index = url.LastIndexOf("/");
            var result = url.Substring(index + 1);

            index = result.IndexOf(".");
            result = result.Substring(0, index);

            return Guid.Parse(result);
        }

        #endregion Private routine
    }
}
