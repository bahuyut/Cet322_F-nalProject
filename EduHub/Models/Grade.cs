namespace EduHub.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AssignmentId { get; set; }
        public double Score { get; set; }
    }
}
