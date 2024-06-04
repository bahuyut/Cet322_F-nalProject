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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Grade(string studentId, List<int> assignmentIds, List<double> scores)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.UserType != "teacher")
            {
                return Forbid();
            }

            if (assignmentIds.Count != scores.Count)
            {
                ModelState.AddModelError("", "Number of assignments does not match number of scores.");
                // Eğer sayılar uyuşmazsa, hata mesajı göster ve öğretmeni tekrar notlandırma sayfasına yönlendir.
                var assignments = await _context.Assignments.ToListAsync();
                ViewBag.Assignments = assignments;
                ViewBag.StudentId = studentId;
                return View();
            }

            for (int i = 0; i < assignmentIds.Count; i++)
            {
                var assignmentId = assignmentIds[i];
                var score = scores[i];

                // Check if the grade already exists for this assignment and student
                var existingGrade = await _context.Grades
                    .FirstOrDefaultAsync(g => g.UserId == studentId && g.AssignmentId == assignmentId);

                if (existingGrade != null)
                {
                    // Update the existing grade
                    existingGrade.Score = score;
                    _context.Update(existingGrade);
                }
                else
                {
                    // Add a new grade
                    var grade = new Grade
                    {
                        UserId = studentId,
                        AssignmentId = assignmentId,
                        Score = score
                    };

                    _context.Add(grade);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TeacherIndex));
        }



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
