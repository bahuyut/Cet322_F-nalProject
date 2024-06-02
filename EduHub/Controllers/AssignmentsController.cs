using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using EduHub.Data;
using EduHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduHub.Controllers
{
    [Authorize]
    public class AssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<EduUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AssignmentsController(ApplicationDbContext context, UserManager<EduUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Assignments
        public async Task<IActionResult> Index()
        {
            var assignments = await _context.Assignments.Include(a => a.EduUser).ToListAsync();
            return View(assignments);
        }

        // GET: Assignments/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.UserType != "teacher")
            {
                return Forbid();
            }
            return View();
        }



        // POST: Assignments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,DueDate")] Assignment assignment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.UserType != "teacher")
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                assignment.EduUserId = user.Id;
                _context.Add(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(assignment);
        }

        // GET: Assignments/Submit/5
        public async Task<IActionResult> Submit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.UserType != "student")
            {
                return Forbid();
            }

            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // POST: Assignments/Submit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int id, IFormFile file)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.UserType != "student")
            {
                return Forbid();
            }

            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            if (file != null && file.Length > 0)
            {
                var uploads = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "assignments");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                var assignmentName = assignment.Title.Replace(" ", "_");
                var userName = user.UserName.Replace(" ", "_");
                var fileName = $"{assignmentName}_{userName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploads, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                assignment.FilePath = "/uploads/assignments/" + fileName;
                _context.Update(assignment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(assignment);
        }



        // GET: Assignments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // GET: Assignments/Download/5
        public async Task<IActionResult> Download(int id)
        {
            var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "assignments");

            if (!Directory.Exists(uploadsFolder))
            {
                return NotFound();
            }

            var files = Directory.GetFiles(uploadsFolder);

            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileName(file);
                        zipArchive.CreateEntryFromFile(file, fileName);
                    }
                }

                return File(memoryStream.ToArray(), "application/zip", "assignments.zip");
            }
        }

        // GET: Assignments/DownloadAll
        public async Task<IActionResult> DownloadAll()
        {
            var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "assignments");

            if (!Directory.Exists(uploadsFolder))
            {
                return NotFound();
            }

            var files = Directory.GetFiles(uploadsFolder);

            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileName(file);
                        zipArchive.CreateEntryFromFile(file, fileName);
                    }
                }

                return File(memoryStream.ToArray(), "application/zip", "assignments.zip");
            }
        }



        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                { ".docx", "application/vnd.ms-word" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" },
                { ".csv", "text/csv" }
            };
        }

        private bool AssignmentExists(int id)
        {
            return _context.Assignments.Any(e => e.Id == id);
        }
    }
}
