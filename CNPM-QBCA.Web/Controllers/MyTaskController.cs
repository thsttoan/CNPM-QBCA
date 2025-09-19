// MyTaskController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBCA.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QBCA.Controllers
{
    public class MyTaskController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MyTaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /MyTask/MyTasks
        public async Task<IActionResult> MyTasks()
        {
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var tasks = await _context.TaskAssignments
                .Include(t => t.ExamPlan)
                .Include(t => t.Distribution)
                .Include(t => t.Assigner)
                .Where(t => t.AssignedTo == userId)
                .OrderByDescending(t => t.DueDate)
                .ToListAsync();

            return View(tasks); // View nằm trong Views/MyTask/MyTasks.cshtml
        }

        // GET: /MyTask/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var task = await _context.TaskAssignments
                .Include(t => t.ExamPlan)
                .Include(t => t.Distribution)
                .Include(t => t.Assigner)
                .FirstOrDefaultAsync(t => t.AssignmentID == id);

            if (task == null)
                return NotFound();

            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (!int.TryParse(userIdClaim, out int userId) || task.AssignedTo != userId)
                return Forbid();

            return View("Details", task);
        }

        // POST: /MyTask/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var task = await _context.TaskAssignments.FindAsync(id);
            if (task == null)
                return NotFound();

            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (!int.TryParse(userIdClaim, out int userId) || task.AssignedTo != userId)
                return Forbid();

            task.Status = newStatus;
            _context.TaskAssignments.Update(task);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Task status updated successfully.";
            return RedirectToAction("MyTasks");
        }
    }
}