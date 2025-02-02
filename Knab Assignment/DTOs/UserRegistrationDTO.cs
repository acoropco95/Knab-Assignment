using Knab.Assignment.API.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Knab.Assignment.API.DTOs
{
    public class UserRegistrationDTO
    {
        [EmailAddress]
        public required string Email { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        [MinimumAge(18)]
        public DateTime DateOfBirth { get; set; }

        public required string Password { get; set; }
    }
}
