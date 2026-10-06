using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Madar.Data;
using Madar.Models;

namespace Madar.Controllers
{
    [Authorize]
    public class LecturesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LecturesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Lectures/Index
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var lectures = await _context.Lectures
                .OrderByDescending(l => l.LectureDate)
                .ToListAsync();

            return View(lectures);
        }

        // GET: Lectures/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lectures/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Lecture lecture)
        {
            if (ModelState.IsValid)
            {
                lecture.CreatedAt = DateTime.Now;
                _context.Lectures.Add(lecture);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(lecture);
        }

        // GET: Lectures/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var lecture = await _context.Lectures.FindAsync(id);

            if (lecture == null) return NotFound();

            return View(lecture);
        }

        // POST: Lectures/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Lecture lecture)
        {
            if (id != lecture.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lecture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LectureExists(lecture.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(lecture);
        }

        // GET: Lectures/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var lecture = await _context.Lectures.FindAsync(id);

            if (lecture == null) return NotFound();

            return View(lecture);
        }

        // POST: Lectures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lecture = await _context.Lectures.FindAsync(id);

            if (lecture != null)
            {
                _context.Lectures.Remove(lecture);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        // GET: Lectures/Details/5
        // عرض تفاصيل محاضرة مع فيديو
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecture = await _context.Lectures.FindAsync(id);

            if (lecture == null)
            {
                return NotFound();
            }

            // جلب عدد الحضور لهذه المحاضرة
            ViewBag.AttendanceCount = await _context.Attendances
                .CountAsync(a => a.LectureId == id);

            return View(lecture);
        }
        // GET: Lectures/Search
        // البحث عن محاضرة بالعنوان أو الوصف
        [AllowAnonymous]
        public async Task<IActionResult> Search(string searchTerm)
        {
            var lectures = from l in _context.Lectures select l;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                lectures = lectures.Where(l =>
                    l.Title.Contains(searchTerm) ||
                    l.Description.Contains(searchTerm));
            }

            ViewBag.SearchTerm = searchTerm;

            var result = await lectures.OrderByDescending(l => l.LectureDate).ToListAsync();

            return View("Index", result);
        }
        private bool LectureExists(int id)
        {
            return _context.Lectures.Any(e => e.Id == id);
        }
    }
}