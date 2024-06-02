namespace EduHub.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public string? UserId { get; set; }  
        public int AssignmentId { get; set; }
        public double Score { get; set; }

        // Navigasyon özelliği
        public Assignment? Assignment { get; set; }
    }
}
