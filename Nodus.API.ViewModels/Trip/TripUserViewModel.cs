using Nodus.API.Models.Role;

namespace Nodus.API.Models.Trip
{
    public class TripUserViewModel
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public RoleViewModel Role { get; set; }

        public string Address { get; set; }

        public string Notes { get; set; }

        public bool IsActive { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsParticipant { get; set; }
        public double? Budget { get; set; }
    }
}
