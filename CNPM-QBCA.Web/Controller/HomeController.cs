using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Security.Claims;
using QBCA.Models;
using System.Linq;
using System.Collections.Generic;
using QBCA.Data;

namespace QBCA.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var roleIdClaim = User.FindFirst("RoleID")?.Value;
            if (!int.TryParse(roleIdClaim, out var roleId))
                return RedirectToAction("AccessDenied");

            return roleId switch
            {
                1 => RedirectToAction("Home_RD"),
                2 => RedirectToAction("Home_Lecturer"),
                3 => RedirectToAction("Home_HeadDept"),
                4 => RedirectToAction("Home_SubjectLeader"),
                5 => RedirectToAction("Home_ExamHead"),
                _ => RedirectToAction("AccessDenied")
            };
        }

        private bool IsRoleAllowed(int expectedRoleId)
        {
            var roleIdClaim = User.FindFirst("RoleID")?.Value;
            if (!int.TryParse(roleIdClaim, out var roleId))
                return false;
            return roleId == expectedRoleId;
        }

        public IActionResult Home_RD()
        {
            if (!IsRoleAllowed(1))
                return RedirectToAction("AccessDenied");
            ViewBag.RoleKey = "rd-staff";
            return View("Home_RD");
        }

        public IActionResult Home_Lecturer()
        {
            if (!IsRoleAllowed(2))
                return RedirectToAction("AccessDenied");
            ViewBag.RoleKey = "lecturer";
            return View("Home_Lecturer");
        }

        public IActionResult Home_HeadDept()
        {
            if (!IsRoleAllowed(3))
                return RedirectToAction("AccessDenied");
            ViewBag.RoleKey = "head-dept";
            return View("Home_HeadDept");
        }

        public IActionResult Home_SubjectLeader()
        {
            if (!IsRoleAllowed(4))
                return RedirectToAction("AccessDenied");
            ViewBag.RoleKey = "subject-leader";
            return View("Home_SubjectLeader");
        }

        public IActionResult Home_ExamHead()
        {
            if (!IsRoleAllowed(5))
                return RedirectToAction("AccessDenied");
            ViewBag.RoleKey = "exam-head";
            return View("Home_ExamHead");
        }

        public IActionResult About()
        {
            ViewBag.Title = "About";
            ViewBag.Message = "This is an introduction page for QBCA.";
            return View();
        }

        public IActionResult Unauthorized()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            ViewBag.Message = "Hey You!!!";
            return View();
        }

        public IActionResult Profile()
        {
            var email = User.Identity?.Name;
            var fullName = User.FindFirst("FullName")?.Value ?? "Unknown";
            var roleId = User.FindFirst("RoleID")?.Value ?? "N/A";

            ViewBag.Email = email;
            ViewBag.FullName = fullName;
            ViewBag.RoleID = roleId;

            return View();
        }

        public IActionResult Logins()
        {
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return RedirectToAction("Unauthorized");

            var logins = _context.Logins
                .Where(l => l.UserID == userId)
                .OrderByDescending(l => l.LastLogin)
                .Take(5)
                .ToList();

            return View(logins);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}