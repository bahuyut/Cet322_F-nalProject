using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EduHub.Models;
using Microsoft.AspNetCore.Identity;

namespace EduHub.Data;

public class ApplicationDbContext : IdentityDbContext<EduUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<EduUser> EduUsers { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<StudentQuiz> StudentQuizzes { get; set; }
    

    
}

