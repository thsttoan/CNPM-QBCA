using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBCA.Data;
using QBCA.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace QBCA.Controllers
{
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Thêm thông báo cho người dùng
        private void AddNotification(int userId, string message, string relatedType, int? relatedId, int createdBy)
        {
            var noti = new Notification
            {
                UserID = userId,
                Message = message,
                Status = "unread",
                RelatedEntityType = relatedType,
                RelatedEntityID = relatedId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };
            _context.Notifications.Add(noti);
        }

        // GET: /Task/Create
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new TaskAssignmentCreateViewModel
            {
                AllUsers = _context.Users.Where(u => u.RoleID == 2).ToList()
            };
            return View(vm);
        }
        // POST: /Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskAssignmentCreateViewModel vm)
        {
            if (vm.DueDate < DateTime.Today)
            {
                ModelState.AddModelError("DueDate", "Due Date must be today or later.");
            }

            if (!ModelState.IsValid)
            {
                vm.AllUsers = _context.Users.Where(u => u.RoleID == 2).ToList();
                return View(vm);
            }

            var userIdClaim = User.FindFirst("UserID")?.Value;
            int createdBy = 0;
            if (int.TryParse(userIdClaim, out int userId))
                createdBy = userId;

            // Xử lý ép kiểu int? -> int cho ExamPlanID
            int examPlanId = vm.ExamPlanID ?? 0;

            var task = new TaskAssignment
            {
                ExamPlanID = examPlanId,
                AssignedBy = createdBy,
                AssignedTo = vm.AssignedTo,
                Role = vm.Role,
                Description = vm.Description,
                TaskType = vm.TaskType,
                Status = vm.Status,
                AssignedAt = DateTime.UtcNow,
                DueDate = vm.DueDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.TaskAssignments.Add(task);
            await _context.SaveChangesAsync();

            var notifyUsers = _context.Users.Where(u => u.RoleID == 2).ToList();
            foreach (var user in notifyUsers)
            {
                AddNotification(user.UserID, "New task assigned to you", "TaskAssignment", task.AssignmentID, createdBy);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Task has been successfully assigned.";

            return RedirectToAction("AssignToLecturers");
        }

        // GET: /Task/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.TaskAssignments.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            var vm = new TaskAssignmentCreateViewModel
            {
                AssignmentID = task.AssignmentID,
                ExamPlanID = task.ExamPlanID,
                AssignedTo = task.AssignedTo,
                Role = task.Role,
                Description = task.Description,
                TaskType = task.TaskType,
                Status = task.Status,
                DueDate = task.DueDate
            };
            vm.AllUsers = _context.Users.ToList();
            return View(vm);
        }
        // POST: /Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskAssignmentCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AllUsers = _context.Users.ToList();
                return View(vm);
            }

            var task = await _context.TaskAssignments.FindAsync(vm.AssignmentID);
            if (task == null)
            {
                return NotFound();
            }

            // Xử lý ép kiểu int? -> int cho ExamPlanID
            int examPlanId = vm.ExamPlanID ?? 0;

            task.ExamPlanID = examPlanId;
            task.AssignedTo = vm.AssignedTo;
            task.Role = vm.Role;
            task.Description = vm.Description;
            task.TaskType = vm.TaskType;
            task.Status = vm.Status;
            task.DueDate = vm.DueDate;

            _context.TaskAssignments.Update(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("Tasks");
        }

        // GET: /Task/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.TaskAssignments.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }
        // POST: /Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.TaskAssignments.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            _context.TaskAssignments.Remove(task);
            await _context.SaveChangesAsync();
            return RedirectToAction("Tasks");
        }

        // CHO SUBJECT LEADER XEM CÁC NHIỆM VỤ ĐƯỢC GIAO CHO MÌNH
        public async Task<IActionResult> TeamTasks()
        {
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var tasks = await _context.TaskAssignments
                .Include(t => t.Assigner)
                .Include(t => t.ExamPlan)
                .Where(t => t.AssignedTo == userId)
                .OrderByDescending(t => t.DueDate)
                .ToListAsync();

            return View("TeamTasks", tasks); // Views/Task/TeamTasks.cshtml
        }

        // CHO SUBJECT LEADER XEM CÁC NHIỆM VỤ MÌNH ĐÃ GIAO
        public async Task<IActionResult> AssignToLecturers()
        {
            var userIdClaim = User.FindFirst("UserID")?.Value;
            int userId = 0;
            if (int.TryParse(userIdClaim, out int parsedUserId))
                userId = parsedUserId;
            var tasks = await _context.TaskAssignments
                .Include(t => t.Assignee)
                .Include(t => t.ExamPlan)
                .Where(t => t.AssignedBy == userId)
                .ToListAsync();
            return View("AssignToLecturers", tasks); // Views/Task/AssignToLecturers.cshtml
        }
    }
}