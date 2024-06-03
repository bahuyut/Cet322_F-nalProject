using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EduHub.Models
{
    public class StudentQuiz
    {
        public int Id { get; set; }
        public string? StudentId { get; set; }
        public EduUser? Student { get; set; }
        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }
        public double Score { get; set; }
    }
}
