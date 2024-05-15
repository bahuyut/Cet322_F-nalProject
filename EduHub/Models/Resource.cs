using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduHub.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string? UploaderName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FilePath { get; set; } //TODO tgbgftft

        [NotMapped]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }
    }
}
