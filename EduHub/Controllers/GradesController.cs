using System.Linq;
using EduHub.Data;
using EduHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduHub.Controllers
{
    [Authorize(Roles = "teacher")]
    public class GradesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<EduUser> _userManager;

        public GradesController(ApplicationDbContext context, UserManager<EduUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Grades
        public IActionResult Index()
        {
            var students = _userManager.Users.Where(u => u.UserType == "student").ToList();
            return View(students);
        }

        // GET: Grades/Grade/5
        public IActionResult Grade(string studentId)
        {
            var assignments = _context.Assignments.ToList();
            ViewBag.Assignments = assignments;
            ViewBag.StudentId = studentId;
            return View();
        }

        // POST: Grades/Grade/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Grade(string studentId, int assignmentId, double score)
        {
            var grade = new Grade
            {
                UserId = int.Parse(studentId),
                AssignmentId = assignmentId,
                Score = score
            };
            _context.Add(grade);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
