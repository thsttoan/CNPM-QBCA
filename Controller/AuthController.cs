using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using QBCA.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using QBCA.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace QBCA.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Email or password is incorrect.");
                return View(model);
            }

            var roleId = user.RoleID;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("FullName", user.FullName),
                new Claim("RoleID", roleId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // ➤ Chuyển trực tiếp đến dashboard tương ứng với vai trò
            return roleId switch
            {
                1 => RedirectToAction("Home_RD", "Home"),
                2 => RedirectToAction("Home_HeadDept", "Home"),
                3 => RedirectToAction("Home_SubjectLeader", "Home"),
                4 => RedirectToAction("Home_Lecturer", "Home"),
                5 => RedirectToAction("Home_ExamHead", "Home"),
                _ => RedirectToAction("Unauthorized", "Home")
            };
        }

        // GET: /Auth/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
    }
}
