using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EduHub.Models
{
    public class EduUser : IdentityUser
    {
        public string? Name { get; set; }

        public string? Role { get; set; }
    }
}
