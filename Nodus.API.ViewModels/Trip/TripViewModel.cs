using Nodus.API.Models.User;
using Nodus.Database.Models.Customer;
using System;

namespace Nodus.API.Models.Trip
{
    public class TripViewModel
    {
        public TripViewModel()
        {
            
        }
        public TripViewModel(int id, string name, string description, double? budget, UserViewModel createdBy, DateTime dateTimeCreated, DateTime dateTimeStart, DateTime dateTimeEnd, TripStatusEnum tripStatus)
        {
            Id = id;
            Name = name;
            Description = description;
            Budget = budget;
            CreatedBy = createdBy;
            DateTimeCreated = dateTimeCreated;
            DateTimeStart = dateTimeStart;
            DateTimeEnd = dateTimeEnd;
            TripStatus = tripStatus;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double? Budget { get; set; }
        public UserViewModel CreatedBy { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public DateTime DateTimeStart { get; set; }
        public DateTime DateTimeEnd { get; set; }
        public TripStatusEnum TripStatus { get; set; }
    }
}
