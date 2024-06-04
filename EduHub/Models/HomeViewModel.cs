using System;
using System.Collections.Generic;

namespace EduHub.Models
{
    public class HomeViewModel
    {
        public string? UserName { get; set; }
        public List<Announcement>? Announcements { get; set; }
        public DateTime CurrentDateTime { get; set; } 
    }
}
