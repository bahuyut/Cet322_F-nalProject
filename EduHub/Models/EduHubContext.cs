using Humanizer.Localisation;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace EduHub.Models
{
    public class EduHubContext : DbContext
    {
        public EduHubContext(DbContextOptions<EduHubContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Grade> Grades { get; set; }
    }
}
