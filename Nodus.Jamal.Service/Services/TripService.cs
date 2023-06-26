using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Nodus.Database.Context;
using Nodus.Database.Models.Customer;
using Nodus.Jamal.Service.Protos;
using System;
using System.ComponentModel;
using Empty = Nodus.Jamal.Service.Protos.Empty;
using Enum = System.Enum;
using Role = Nodus.Jamal.Service.Protos.Role;
using Trip = Nodus.Jamal.Service.Protos.Trip;

namespace Nodus.Jamal.Service.Services
{
    public class TripService
    {
        private readonly AdminContext _adminContext;
        private readonly EFContextFactory _efContextFactory;

        public TripService(AdminContext adminContext, EFContextFactory efContextFactory)
        {
            _adminContext = adminContext;
            _efContextFactory = efContextFactory;
        }

        public async Task<GetTripResponse> GetTripByIdAsync(GetTripRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var result = await clientContext.Trips
                .Include(trip => trip.Creator)
                .Select(trip => new GetTripResponse()
                {
                    Trip = new Trip()
                    {
                        Id = trip.Id,
                        Name = trip.Name,
                        Description = trip.Description,
                        Budget = trip.Budget,
                        DateTimeCreated = Timestamp.FromDateTime(trip.DateTimeCreated.ToUniversalTime()),
                        DateTimeEnd = Timestamp.FromDateTime(trip.EndOfTrip.ToUniversalTime()),
                        DateTimeStart = Timestamp.FromDateTime(trip.StartOfTrip.ToUniversalTime()),
                        CreatedBy = new ExtendedUserData()
                        {
                            Id = trip.Creator.Id.ToString(),
                            Address = trip.Creator.Address,
                            Email = trip.Creator.Email,
                            FirstName = trip.Creator.FirstName,
                            LastName = trip.Creator.LastName,
                            Notes = trip.Creator.Notes,
                            PhoneNumber = trip.Creator.PhoneNumber
                        },
                        TripStatus = trip.Status.ToString()
                    }
                })
                .FirstAsync(trip => trip.Trip.Id == request.TripId);

            var creator = await _adminContext.Users
                .Include(user => user.Role)
                    .ThenInclude(role => role.RoleFeatures)
                        .ThenInclude(roleFeature => roleFeature.Feature)
                .FirstAsync(user => user.Id == Guid.Parse(result.Trip.CreatedBy.Id));

            result.Trip.CreatedBy.Role = new Role()
            {
                Id = creator.Role.Id,
                Name = creator.Role.Name,
                Description = creator.Role.Description,
                Features =
                {
                    creator.Role.RoleFeatures.Select(feature => new Protos.Feature()
                    {
                        Id = feature.Feature.Id,
                        Description = feature.Feature.Description,
                        Name = feature.Feature.Name
                    })
                }
            };

            return result;
        }

        public async Task<GetUserTripsResponse> GetUserTripsAsync(GetUserTripsRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var requestedStatus = (TripStatusEnum)System.Enum.Parse(typeof(TripStatusEnum), request.TripStatus);

            IQueryable<Database.Models.Customer.Trip> tripsModelsQuery = clientContext.Trips
                .Include(trip => trip.UserTrips)
                    .ThenInclude(participant => participant.User)
                .Include(trip => trip.Creator)
                .Where(trip => trip.Status == requestedStatus);

            switch (request.TripRole)
            {
                case TripRole.All:
                    tripsModelsQuery = tripsModelsQuery
                        .Where(trip => trip.UserTrips.Select(participant => participant.User.Id).Contains(Guid.Parse(request.UserId))
                            || trip.CreatorId == Guid.Parse(request.UserId));
                    break;

                case TripRole.Participant:
                    tripsModelsQuery = tripsModelsQuery
                        .Where(trip => trip.UserTrips.Any(user => user.UserId == Guid.Parse(request.UserId) && user.CanUploadBills));
                    break;

                case TripRole.NonParticipant:
                    tripsModelsQuery = tripsModelsQuery
                        .Where(trip => trip.UserTrips.Any(user => user.UserId == Guid.Parse(request.UserId) && !user.CanUploadBills));
                    break;

                case TripRole.Owner:
                    tripsModelsQuery = tripsModelsQuery
                        .Where(trip => trip.CreatorId == Guid.Parse(request.UserId));
                    break;

                default:
                    throw new NotImplementedException("Use of not implemented trip role");
            }

            var totalCount = await tripsModelsQuery.CountAsync();

            var trips = await tripsModelsQuery
                .OrderBy(trip => trip.DateTimeCreated)
                .Skip(request.Pagination.Offset)
                .Take(request.Pagination.Limit)
                .Select(trip => new Trip
                {
                    Id = trip.Id,
                    Name = trip.Name,
                    Description = trip.Description,
                    Budget = trip.Budget,
                    DateTimeCreated = Timestamp.FromDateTime(trip.DateTimeCreated.ToUniversalTime()),
                    DateTimeEnd = Timestamp.FromDateTime(trip.EndOfTrip.ToUniversalTime()),
                    DateTimeStart = Timestamp.FromDateTime(trip.StartOfTrip.ToUniversalTime()),
                    CreatedBy = new ExtendedUserData()
                    {
                        Id = trip.Creator.Id.ToString(),
                        Address = trip.Creator.Address,
                        Email = trip.Creator.Email,
                        FirstName = trip.Creator.FirstName,
                        LastName = trip.Creator.LastName,
                        Notes = trip.Creator.Notes,
                        PhoneNumber = trip.Creator.PhoneNumber
                    },
                    TripStatus = trip.Status.ToString()
                })
                .ToListAsync();

            var creators = await _adminContext.Users
                .Include(user => user.Role)
                    .ThenInclude(role => role.RoleFeatures)
                        .ThenInclude(roleFeature => roleFeature.Feature)
                .Where(user => trips.Select(trip => Guid.Parse(trip.CreatedBy.Id)).Contains(user.Id))
                .ToListAsync();

            foreach (var trip in trips)
            {
                var creator = creators.First(creator => creator.Id == Guid.Parse(trip.CreatedBy.Id));

                trip.CreatedBy.Role = new Role()
                {
                    Id = creator.Role.Id,
                    Name = creator.Role.Name,
                    Description = creator.Role.Description,
                    Features =
                {
                    creator.Role.RoleFeatures.Select(feature => new Protos.Feature()
                    {
                        Id = feature.Feature.Id,
                        Description = feature.Feature.Description,
                        Name = feature.Feature.Name
                    })
                }
                };
            }

            var result = new GetUserTripsResponse()
            {
                Trips = { trips },
                Pagination = new PaginationResponse()
                {
                    TotalCount = totalCount
                }
            };

            return result;
        }

        public async Task<GetTripUsersResponse> GetTripUsersAsync(GetTripUsersRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var totalCount = await clientContext.UserDetails
                .Include(user => user.UserTrips)
                .Where(user => user.UserTrips.Any(trip => trip.TripId == request.TripId))
                .CountAsync();

            var usersDetails = await clientContext.UserDetails
                .Include(user => user.UserTrips)
                .Where(user => user.UserTrips.Any(trip => trip.TripId == request.TripId))
                .OrderBy(user => user.Email)
                .Skip(request.Pagination.Offset)
                .Take(request.Pagination.Limit)
                .ToListAsync();

            var users = await _adminContext.Users
                .Include(user => user.Role)
                    .ThenInclude(role => role.RoleFeatures)
                        .ThenInclude(roleFeature => roleFeature.Feature)
                .Where(user => usersDetails.Select(details => details.Id).Contains(user.Id))
                .OrderBy(user => user.Email)
                .ToListAsync();

            //Create users models
            var usersModels = new List<TripUser>();

            foreach (var user in users)
            {
                var userDetails = usersDetails.First(details => details.Id == user.Id);

                var userModel = new TripUser()
                {
                    Id = userDetails.Id.ToString(),
                    Address = userDetails.Address,
                    Email = userDetails.Email,
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    Notes = userDetails.Notes,
                    PhoneNumber = userDetails.PhoneNumber,
                    IsActive = user.IsActive,
                    IsParticipant = userDetails.UserTrips.First(trip => trip.TripId == request.TripId).CanUploadBills,
                    Budget = userDetails.UserTrips.First(trip => trip.TripId == request.TripId).Budget,
                    Role = new Role()
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
                    }
                };

                usersModels.Add(userModel);
            }

            var result = new GetTripUsersResponse()
            {
                Users = { usersModels },
                Pagination = new PaginationResponse()
                {
                    TotalCount = totalCount
                }
            };

            return result;
        }

        public async Task<CreateTripResponse> CreateTripAsync(CreateTripRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var trip = new Database.Models.Customer.Trip()
            {
                Name = request.Name,
                DateTimeCreated = DateTime.UtcNow,
                Description = request.Description,
                EndOfTrip = request.DateTimeEnd.ToDateTime(),
                StartOfTrip = request.DateTimeStart.ToDateTime(),
                CreatorId = Guid.Parse(request.CreatedById),
                Budget = request.Budget
            };

            await clientContext.Trips.AddAsync(trip);

            List<UserTrip> userTrips = new List<UserTrip>();
            foreach (var user in request.Users)
            {
                var userTrip = new UserTrip()
                {
                    CanUploadBills = user.CanUploadBills,
                    UserId = Guid.Parse(user.UserId),
                    Trip = trip,
                    Budget = user.Budget
                };

                userTrips.Add(userTrip);
            }

            await clientContext.UserTrips.AddRangeAsync(userTrips);

            await clientContext.SaveChangesAsync();

            return new CreateTripResponse()
            {
                TripId = trip.Id
            };
        }

        public async Task<Empty> UpdateTripDataAsync(UpdateTripDataRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var trip = await clientContext.Trips
                .Include(trip => trip.UserTrips)
                .FirstAsync(trip => trip.Id == request.TripId);

            trip.Name = request.Name;
            trip.Description = request.Description;
            trip.Budget = request.Budget;
            trip.StartOfTrip = request.DateTimeStart.ToDateTime();
            trip.EndOfTrip = request.DateTimeEnd.ToDateTime();

            await clientContext.SaveChangesAsync();

            return new Empty();
        }

        public async Task<Empty> DeleteTripAsync(DeleteTripRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var trip = await clientContext.Trips.FirstAsync(trip => trip.Id == request.TripId);

            clientContext.Trips.Remove(trip);

            await clientContext.SaveChangesAsync();

            return new Empty();
        }        

        public async Task<Empty> ChangeTripStatusAsync(ChangeTripStatusRequest request)
        {
            if (!Enum.IsDefined(typeof(TripStatusEnum), request.StatusId))
            {
                throw new ArgumentException("StatusId is wrong");
            }

            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var trip = await clientContext.Trips.FirstAsync(trip => trip.Id == request.TripId);

            trip.Status = (TripStatusEnum)request.StatusId;

            await clientContext.SaveChangesAsync();

            return new Empty();
        }

        public async Task<Empty> UpdateTripUsersAsync(UpdateTripUsersRequest request)
        {
            var clientContext = await _efContextFactory.GetRequiredContext(request.CompanyId);

            var trip = await clientContext.Trips.Include(s => s.UserTrips).FirstAsync(trip => trip.Id == request.TripId);

            IEnumerable<UserTrip> userTrips = request.Users.Select(user => new UserTrip()
                {
                    CanUploadBills = user.CanUploadBills,
                    UserId = Guid.Parse(user.UserId),
                    Trip = trip,
                    Budget = user.Budget
                });

            var userTripsToDelete = trip.UserTrips.Where(t => !userTrips.Any(ut => ut.TripId == t.TripId && ut.UserId == t.UserId)).ToList();
            var userTripsToAdd = userTrips.Where(t => !trip.UserTrips.Any(ut => ut.TripId == t.TripId && ut.UserId == t.UserId)).ToList();

            userTripsToDelete.ForEach(ut => trip.UserTrips.Remove(ut));
            userTripsToAdd.ForEach(ut => trip.UserTrips.Add(ut));

            await clientContext.SaveChangesAsync();

            return new Empty();
        }
    }
}
