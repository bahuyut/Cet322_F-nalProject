using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EduHub.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System;
using EduHub.Data;

namespace EduHub.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<EduUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(UserManager<EduUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var userName = user.Name ?? "Kullanıcı";
            var announcements = _context.Announcements.OrderByDescending(a => a.PostedDate).Take(3).ToList();

            var model = new HomeViewModel
            {
                UserName = userName,
                Announcements = announcements,
                CurrentDateTime = DateTime.Now // Şu anki tarih ve saat bilgisi
            };

            return View(model);
        }
    }
}
