using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduHub.Data;
using EduHub.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EduHub.Controllers
{
    [Authorize]
    public class QuizzesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<EduUser> _userManager;

        public QuizzesController(ApplicationDbContext context, UserManager<EduUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Quizzes
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Forbid();
            }

            if (user.UserType == "teacher")
            {
                var quizzes = await _context.Quizzes
                    .Where(q => q.TeacherId == user.Id)
                    .ToListAsync();
                return View("TeacherIndex", quizzes);
            }
            else if (user.UserType == "student")
            {
                var quizzes = await _context.Quizzes
                    .Include(q => q.StudentQuizzes)
                    .ToListAsync();
                return View("StudentIndex", quizzes);
            }

            return Forbid();
        }


        // GET: Quizzes/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.UserType != "teacher")
            {
                return Forbid();
            }

            return View();
        }

        // POST: Quizzes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Questions")] Quiz quiz)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.UserType != "teacher")
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                quiz.TeacherId = user.Id;
                _context.Add(quiz);
                await _context.SaveChangesAsync();

                // Yeni duyuru oluştur
                var announcement = new Announcement
                {
                    UploaderName = user.Name,
                    Title = "Yeni Quiz Eklendi",
                    Content = "Yeni bir quiz eklendi: " + quiz.Title,
                    PostedDate = DateTime.Now
                };

                _context.Add(announcement);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(quiz);
        }


        // GET: Quizzes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .Include(q => q.StudentQuizzes)
                    .ThenInclude(sq => sq.Student) // Student nesnesini de dahil ediyoruz
                .FirstOrDefaultAsync(m => m.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Forbid();
            }

            if (user.UserType == "teacher" && quiz.TeacherId == user.Id)
            {
                return View("TeacherDetails", quiz);
            }
            else if (user.UserType == "student")
            {
                var studentQuiz = quiz.StudentQuizzes?.FirstOrDefault(sq => sq.StudentId == user.Id);
                if (studentQuiz != null)
                {
                    ViewBag.Score = studentQuiz.Score;
                    return View("StudentDetails", quiz);
                }
                return View("StudentSolve", quiz);
            }

            return Forbid();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Quizzes/Solve/{id}")]
        public async Task<IActionResult> Solve(int id, [Bind("Answers")] QuizSolveViewModel solveViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.UserType != "student")
            {
                return Forbid();
            }

            var quiz = await _context.Quizzes.Include(q => q.Questions).FirstOrDefaultAsync(q => q.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }

            double score = 0;
            foreach (var question in quiz.Questions)
            {
                if (solveViewModel.Answers.TryGetValue(question.Id, out string answer) && answer == question.CorrectOption)
                {
                    score += 1;
                }
            }
            score = (score / quiz.Questions.Count) * 100;

            var studentQuiz = new StudentQuiz
            {
                StudentId = user.Id,
                QuizId = quiz.Id,
                Score = score,
                Student = user // Öğrenci nesnesini ekliyoruz
            };

            _context.StudentQuizzes.Add(studentQuiz);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }





        public class QuizSolveViewModel
        {
            public Dictionary<int, string> Answers { get; set; }
        }
    }
}
