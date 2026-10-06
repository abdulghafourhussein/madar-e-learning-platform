using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Madar.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Madar.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // إحصائيات عامة
            ViewBag.TotalLectures = await _context.Lectures.CountAsync();
            ViewBag.TotalStudents = await _context.Users.CountAsync();

            // عدد الحضور لكل محاضرة
            var attendancePerLecture = await _context.Attendances
                .Include(a => a.Lecture)
                .GroupBy(a => new { a.LectureId, a.Lecture.Title })
                .Select(g => new
                {
                    LectureTitle = g.Key.Title,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return View(attendancePerLecture);
        }
    }
}