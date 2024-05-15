
using System;

namespace EduHub.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FilePath { get; set; }
        public DateTime DueDate { get; set; }

        public string? EduUserId { get; set; }
        public virtual EduUser? EduUser { get; set; }
    }
}
