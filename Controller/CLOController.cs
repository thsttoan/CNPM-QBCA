using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBCA.Data;
using QBCA.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QBCA.Controllers
{
    public class CLOController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CLOController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /CLO/CLOs
        public async Task<IActionResult> CLOs()
        {
            var list = await _context.CLOs
                .Include(c => c.Subject)
                .Include(c => c.Questions)
                .ToListAsync();
            return View(list);
        }

        // GET: /CLO/Create
        public IActionResult Create()
        {
            var vm = new CLOCreateViewModel
            {
                AllSubjects = _context.Subjects.ToList(),
                AllQuestions = _context.Questions.ToList()
            };
            return View(vm);
        }

        // POST: /CLO/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CLOCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var clo = new CLO
                {
                    Code = vm.Code,
                    Description = vm.Description,
                    SubjectID = vm.SubjectID.Value,
                    Questions = new List<Question>()
                };

                if (vm.SelectedQuestionIDs != null && vm.SelectedQuestionIDs.Count > 0)
                {
                    clo.Questions = await _context.Questions
                        .Where(q => vm.SelectedQuestionIDs.Contains(q.QuestionID))
                        .ToListAsync();
                }

                _context.CLOs.Add(clo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "CLO created successfully!";
                return RedirectToAction("CLOs");
            }

            vm.AllSubjects = _context.Subjects.ToList();
            vm.AllQuestions = _context.Questions.ToList();
            return View(vm);
        }

        // GET: /CLO/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var clo = await _context.CLOs
                .Include(c => c.Questions)
                .FirstOrDefaultAsync(c => c.CLOID == id);

            if (clo == null)
            {
                return NotFound();
            }

            var vm = new CLOCreateViewModel
            {
                CLOID = clo.CLOID,
                Code = clo.Code,
                Description = clo.Description,
                SubjectID = clo.SubjectID,
                SelectedQuestionIDs = clo.Questions?.Select(q => q.QuestionID).ToList() ?? new List<int>(),
                AllSubjects = _context.Subjects.ToList(),
                AllQuestions = _context.Questions.ToList()
            };

            return View(vm);
        }

        // POST: /CLO/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CLOCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AllSubjects = _context.Subjects.ToList();
                vm.AllQuestions = _context.Questions.ToList();
                return View(vm);
            }

            var clo = await _context.CLOs
                .Include(c => c.Questions)
                .FirstOrDefaultAsync(c => c.CLOID == vm.CLOID);

            if (clo == null)
            {
                return NotFound();
            }

            clo.Code = vm.Code;
            clo.Description = vm.Description;
            clo.SubjectID = vm.SubjectID.Value;

            // Cập nhật danh sách câu hỏi
            clo.Questions.Clear();
            if (vm.SelectedQuestionIDs != null && vm.SelectedQuestionIDs.Count > 0)
            {
                var selectedQuestions = await _context.Questions
                    .Where(q => vm.SelectedQuestionIDs.Contains(q.QuestionID))
                    .ToListAsync();
                foreach (var q in selectedQuestions)
                {
                    clo.Questions.Add(q);
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "CLO updated successfully!";
            return RedirectToAction("CLOs");
        }
    }
}