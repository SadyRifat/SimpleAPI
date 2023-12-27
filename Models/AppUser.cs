using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleAPI.Models
{
    public class AppUser : IdentityUser
    {
        public string? ProfileImageUrl { get; set; }
    }
}
