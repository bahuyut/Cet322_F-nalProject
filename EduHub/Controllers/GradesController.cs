using System.Linq;
using System.Threading.Tasks;
using EduHub.Data;
using EduHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduHub.Controllers
{
    public class GradesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<EduUser> _userManager;

        public GradesController(ApplicationDbContext context, UserManager<EduUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Grades/Index
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Forbid();
            }

            if (user.UserType == "teacher")
            {
                return RedirectToAction(nameof(TeacherIndex));
            }
            else if (user.UserType == "student")
            {
                return RedirectToAction(nameof(StudentIndex));
            }

            return Forbid();
        }

        // GET: Grades/TeacherIndex
        public async Task<IActionResult> TeacherIndex()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.UserType != "teacher")
            {
                return Forbid();
            }

            var students = await _userManager.Users.Where(u => u.UserType == "student").ToListAsync();
            return View(students);
        }

        // GET: Grades/Grade/5
        public async Task<IActionResult> Grade(string studentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.UserType != "teacher")
            {
                return Forbid();
            }

            var assignments = await _context.Assignments.ToListAsync();
            ViewBag.Assignments = assignments;
            ViewBag.StudentId = studentId;
            return View();
        }

        // POST: Grades/Grade/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Grade(string studentId, int assignmentId, double score)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.UserType != "teacher")
            {
                return Forbid();
            }

            var grade = new Grade
            {
                UserId = studentId,  // GUID formatındaki string türü
                AssignmentId = assignmentId,
                Score = score
            };

            _context.Add(grade);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TeacherIndex));
        }

        // GET: Grades/StudentIndex
        public async Task<IActionResult> StudentIndex()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.UserType != "student")
            {
                return Forbid();
            }

            var grades = await _context.Grades
                .Where(g => g.UserId == user.Id)
                .Include(g => g.Assignment)
                .ToListAsync();
            return View(grades);
        }
    }
}
