using CNPM_QBCA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBCA.Data;



public class ExamApproveTaskController : Controller
{
    private readonly ApplicationDbContext _context;
    private int currentUserId;

    public ExamApproveTaskController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userIdStr = User.FindFirst("UserID")?.Value;
        if (!int.TryParse(userIdStr, out int currentUserId))
            return RedirectToAction("Unauthorized", "Home");

        var tasks = await _context.ExamApproveTasks
            .Include(e => e.Exam)
            .Include(e => e.AssignedBy)
            .Where(t => t.AssignedToUserID == currentUserId) // hoặc để hiển thị hết nếu là admin
            .ToListAsync();

        var viewModel = tasks.Select(t => new ApproveExamTaskViewModel
        {
            TaskID = t.ExamApproveTaskID,
            ExamTitle = t.Exam.Title,
            Status = t.Status,
            AssignedBy = t.AssignedBy.FullName,
            AssignedDate = t.AssignedDate,
            CreatedAt = t.CreatedAt
        }).ToList();

        return View(viewModel);
    }


    public async Task<IActionResult> Details(int id)
    {
        var task = await _context.ExamApproveTasks
            .Include(t => t.Exam)
            .Include(t => t.AssignedBy)
            .Include(t => t.AssignedTo)
            .FirstOrDefaultAsync(t => t.ExamApproveTaskID == id);

        if (task == null) return NotFound();

        // Giả sử mỗi Exam có 1 Submission duy nhất → tìm theo ExamID
        var submission = await _context.SubmissionTables
            .Include(s => s.Questions)
            .Include(s => s.Creator)
            .FirstOrDefaultAsync(s => s.PlanID == task.Exam.ExamPlanID); // hoặc dùng ExamID nếu bạn có mapping riêng

        var viewModel = new ApproveExamDetailViewModel
        {
            Task = task,
            Submission = submission
        };

        return View(viewModel);
    }


    [HttpPost]
    public async Task<IActionResult> Approve(int id, string feedback)
    {
        var task = await _context.ExamApproveTasks.FindAsync(id);
        if (task == null)
            return NotFound();

        task.Status = "Approved";
        task.Feedback = feedback;
        task.ApprovedDate = DateTime.Now;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id, string feedback)
    {
        var task = await _context.ExamApproveTasks.FindAsync(id);
        if (task == null)
            return NotFound();

        task.Status = "Rejected";
        task.Feedback = feedback;
        task.ApprovedDate = DateTime.Now;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
