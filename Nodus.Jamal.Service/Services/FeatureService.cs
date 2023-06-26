using Microsoft.EntityFrameworkCore;
using Nodus.Database.Context;
using Nodus.Jamal.Service.Protos;

namespace Nodus.Jamal.Service.Services
{
    public class FeatureService
    {
        private readonly AdminContext _adminContext;

        public FeatureService(AdminContext adminContext)
        {
            _adminContext = adminContext;
        }

        public async Task<GetFeaturesResponse> GetFeaturesAsync()
        {
            var features = await _adminContext.Features
                .OrderBy(feature => feature.Id)
                .Select(feature => new Feature()
                {
                    Id = feature.Id,
                    Name = feature.Name,
                    Description = feature.Description
                })
                .ToListAsync();

            var response = new GetFeaturesResponse()
            {
                Features = { features }
            };

            return response;
        }
    }
}
