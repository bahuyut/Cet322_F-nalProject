using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EduHub.Models
{
    public class EduUser : IdentityUser
    {
        public string? Name { get; set; }

        public string? UserType { get; set; }

        public string? Department { get; set; }

        public ICollection<Assignment>? Assignments { get; set; }
    }
}
