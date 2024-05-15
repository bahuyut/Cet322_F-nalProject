﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
    //public DbSet<EduUser> EduUsers { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Grade> Grades { get; set; }
}

