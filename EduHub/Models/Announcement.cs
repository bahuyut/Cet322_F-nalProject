using System;

namespace EduHub.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string? UploaderName { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime PostedDate { get; set; }
    }
}
