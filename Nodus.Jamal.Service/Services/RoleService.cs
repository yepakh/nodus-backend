using Microsoft.EntityFrameworkCore;
using Nodus.Database.Context;
using Nodus.Database.Models.Admin;
using Nodus.Jamal.Service.Protos;
using System.Data;
using Role = Nodus.Database.Models.Admin.Role;

namespace Nodus.Jamal.Service.Services
{
    public class RoleService
    {
        private readonly AdminContext _adminContext;

        public RoleService(AdminContext adminContext)
        {
            _adminContext = adminContext;
        }

        public async Task<GetRoleResponse> GetRoleAsync(GetRoleRequest request)
        {
            var role = await _adminContext.Roles
                .Include(role => role.RoleFeatures)
                    .ThenInclude(role => role.Feature)
                .Select(role => new
                {
                    role.Id,
                    role.Name,
                    role.Description,
                    Features =
                    role.RoleFeatures.Select(feature => new Protos.Feature()
                    {
                        Id = feature.Feature.Id,
                        Name = feature.Feature.Name,
                        Description = feature.Feature.Description
                    })
                })
                .FirstAsync(role => role.Id == request.RoleId);

            var result = new GetRoleResponse()
            {
                Role = new Protos.Role()
                {
                    Id = role.Id,
                    Description = role.Description,
                    Name = role.Name,
                    Features = { role.Features }
                }
            };

            return result;
        }

        public async Task<GetRolesResponse> GetRolesAsync(GetRolesRequest request)
        {
            var roles = await _adminContext.Roles
                .Include(role => role.RoleFeatures)
                    .ThenInclude(roleFeature => roleFeature.Feature)
                .Where(role => role.CompanyId == request.CompanyId)
                .OrderBy(role => role.Name)
                .Skip(request.Pagination.Offset)
                .Take(request.Pagination.Limit)
                .Select(role => new
                {
                    role.Id,
                    role.Name,
                    role.Description,
                    Features =
                        role.RoleFeatures
                        .Select(roleFeature => new Protos.Feature()
                        {
                            Id = roleFeature.Feature.Id,
                            Name = roleFeature.Feature.Name,
                            Description = roleFeature.Feature.Description
                        })
                })
                .ToListAsync();

            var totalCount = await _adminContext.Roles
                .Where(role => role.CompanyId == request.CompanyId)
                .CountAsync();

            var result = new GetRolesResponse()
            {
                Pagination = new PaginationResponse()
                {
                    TotalCount = totalCount
                },
                Roles =
                {
                    roles.Select(role => new Protos.Role()
                    {
                        Id =role.Id,
                        Name = role.Name,
                        Description = role.Description,
                        Features = {role.Features }
                    })
                }
            };

            return result;
        }

        public async Task<CreateRoleResponse> CreateRoleAsync(CreateRoleRequest request)
        {
            var role = new Role()
            {
                Name = request.Name,
                Description = request.Description,
                CompanyId = request.CompanyId
            };
            await _adminContext.Roles.AddAsync(role);

            var features = _adminContext.Features
                .Where(feature => request.AvaliableFeaturesIds.Contains(feature.Id))
                .ToList();

            var roleFeatures = new List<RoleFeature>();

            foreach (var feature in features)
            {
                roleFeatures.Add(new RoleFeature()
                {
                    FeatureId = feature.Id,
                    Role = role,
                });
            }

            await _adminContext.RoleFeatures.AddRangeAsync(roleFeatures);
            await _adminContext.SaveChangesAsync();

            return new CreateRoleResponse()
            {
                RoleId = role.Id
            };
        }

        public async Task<Empty> UpdateRoleAsync(UpdateRoleRequest request)
        {
            var role = await _adminContext.Roles
                .Include(role => role.RoleFeatures)
                .FirstAsync(role => role.Id == request.Id);

            role.Name = request.Name;
            role.Description = request.Description;
            role.RoleFeatures.Clear();

            var newFeatures = _adminContext.Features
                .Where(feature => request.AvaliableFeaturesIds.Contains(feature.Id))
                .ToList();

            foreach (var newFeature in newFeatures)
            {
                role.RoleFeatures.Add(new RoleFeature()
                {
                    RoleId = role.Id,
                    FeatureId = newFeature.Id
                });
            }

            await _adminContext.SaveChangesAsync();

            return new Empty();
        }

        public async Task<Empty> DeleteRoleAsync(DeleteRoleRequest request)
        {
            var role = await _adminContext.Roles
                .FirstAsync(role => role.Id == request.Id);
            _adminContext.Roles.Remove(role);
            
            await _adminContext.SaveChangesAsync();

            return new Empty();
        }
    }
}
