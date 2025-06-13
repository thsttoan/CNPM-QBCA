using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBCA.Data;
using QBCA.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QBCA.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubmissionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Submission/SubmissionTable
        public async Task<IActionResult> SubmissionTable()
        {
            var submissions = await _context.SubmissionTables
                .Include(s => s.ExamPlan)  
                .Include(s => s.Creator)
                .Include(s => s.Approver)
                .Include(s => s.Questions)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            return View(submissions);
        }

    }
}