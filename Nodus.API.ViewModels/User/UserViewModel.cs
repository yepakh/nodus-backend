using Nodus.API.Models.Role;

namespace Nodus.API.Models.User
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            
        }
        public UserViewModel(string id, string firstName, string lastName, string email, RoleViewModel role, string address, string notes, bool isActive, string phoneNumber)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
            Address = address;
            Notes = notes;
            IsActive = isActive;
            PhoneNumber = phoneNumber;
        }

        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public RoleViewModel Role { get; set; }

        public string Address { get; set; }

        public string Notes { get; set; }

        public bool IsActive { get; set; }

        public string PhoneNumber { get; set; }
    }
}
