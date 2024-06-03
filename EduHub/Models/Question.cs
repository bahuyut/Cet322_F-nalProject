using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EduHub.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? CorrectOption { get; set; }
        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }
    }
}
