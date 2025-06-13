using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBCA.Data;
using QBCA.Models;

namespace QBCA.Controllers
{
    public class ExamManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ExamManager/ExamManagers
        public async Task<IActionResult> ExamManagers()
        {
            var examPlans = await _context.ExamPlans
                .Include(p => p.Subject)
                .Include(p => p.Distributions)
                    .ThenInclude(d => d.DifficultyLevel)
                .Include(p => p.Distributions)
                    .ThenInclude(d => d.AssignedManagerRole)
                .ToListAsync();

            return View(examPlans);
        }

        // GET: /ExamManager/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var plan = await _context.ExamPlans
                .Include(p => p.Subject)
                .Include(p => p.Distributions)
                    .ThenInclude(d => d.DifficultyLevel)
                .Include(p => p.Distributions)
                    .ThenInclude(d => d.AssignedManagerRole)
                .FirstOrDefaultAsync(p => p.ExamPlanID == id);

            if (plan == null) return NotFound();

            return View(plan);
        }
    }
}