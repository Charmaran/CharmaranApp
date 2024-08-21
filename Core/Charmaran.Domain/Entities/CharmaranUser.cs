using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Charmaran.Domain.Entities
{
    public class CharmaranUser : IdentityUser
    {
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; } = null!;
        
        [Required]
        [MaxLength(255)]
        public string LastName { get; set; } = null!;
    }
}