using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EduHub.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? TeacherId { get; set; }
        public EduUser? Teacher { get; set; }
        public ICollection<Question>? Questions { get; set; }
        public ICollection<StudentQuiz>? StudentQuizzes { get; set; }
    }
}