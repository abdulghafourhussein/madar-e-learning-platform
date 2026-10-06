using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Madar.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        public string StudentId { get; set; }

        public int LectureId { get; set; }

        public DateTime AttendanceDate { get; set; } = DateTime.Now;

        [ForeignKey("StudentId")]
        public IdentityUser Student { get; set; }

        [ForeignKey("LectureId")]
        public Lecture Lecture { get; set; }
    }
}