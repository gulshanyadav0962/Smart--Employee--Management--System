using Microsoft.AspNetCore.Identity;

namespace Smart_Employee_Management_System.Models
{
    public class Customer
    {

        public string? IdentityUserId { get; set; }

        public IdentityUser? IdentityUser { get; set; }
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
    }
}