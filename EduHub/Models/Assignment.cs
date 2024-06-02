using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("EduUserId")]
        public EduUser? EduUser { get; set; }
    }
}
