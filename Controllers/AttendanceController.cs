using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Madar.Data;
using Madar.Models;
using System;
using System.Threading.Tasks;

namespace Madar.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AttendanceController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: Attendance/Register/5
        // يسجل حضور الطالب في محاضرة
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Register(int lectureId)
        {
            // جلب معرف المستخدم الحالي
            var userId = _userManager.GetUserId(User);

            // التحقق أن المحاضرة موجودة
            var lecture = await _context.Lectures.FindAsync(lectureId);
            if (lecture == null)
            {
                return NotFound();
            }

            // التحقق أن الطالب لم يسجل حضوره مسبقاً في هذه المحاضرة
            var alreadyRegistered = await _context.Attendances
                .AnyAsync(a => a.StudentId == userId && a.LectureId == lectureId);

            if (alreadyRegistered)
            {
                TempData["Message"] = "⚠️ لقد سجلت حضورك في هذه المحاضرة مسبقاً.";
                return RedirectToAction("Index", "Lectures");
            }

            // إنشاء سجل حضور جديد
            var attendance = new Attendance
            {
                StudentId = userId,
                LectureId = lectureId,
                AttendanceDate = DateTime.Now
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            TempData["Message"] = "✅ تم تسجيل حضورك بنجاح!";
            return RedirectToAction("Index", "Lectures");
        }
    }
}