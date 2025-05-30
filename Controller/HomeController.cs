using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Security.Claims;
using QBCA.Models;

namespace QBCA.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var roleIdClaim = User.FindFirst("RoleId")?.Value;
            if (!int.TryParse(roleIdClaim, out var roleId))
                return View("Unauthorized");

            return roleId switch
            {
                1 => RedirectToAction("Home_RD"),
                2 => RedirectToAction("Home_HeadDept"),
                3 => RedirectToAction("Home_SubjectLeader"),
                4 => RedirectToAction("Home_Lecturer"),
                5 => RedirectToAction("Home_ExamHead"),
                _ => View("Unauthorized")
            };
        }

        public IActionResult Home_RD()
        {
            ViewBag.RoleKey = "rd-staff";
            return View("Home_RD");
        }

        public IActionResult Home_HeadDept()
        {
            ViewBag.RoleKey = "head-dept";
            return View();
        }

        public IActionResult Home_SubjectLeader()
        {
            ViewBag.RoleKey = "subject-leader";
            return View();
        }

        public IActionResult Home_Lecturer()
        {
            ViewBag.RoleKey = "lecturer";
            return View();
        }

        public IActionResult Home_ExamHead()
        {
            ViewBag.RoleKey = "exam-head";
            return View();
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
        public IActionResult Profile()
        {
            var email = User.Identity?.Name;
            var fullName = User.FindFirst("FullName")?.Value ?? "Unknown";
            var roleId = User.FindFirst("RoleId")?.Value ?? "N/A";

            ViewBag.Email = email;
            ViewBag.FullName = fullName;
            ViewBag.RoleID = roleId;

            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
