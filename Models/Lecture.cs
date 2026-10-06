using System;
using System.ComponentModel.DataAnnotations;

namespace Madar.Models
{
    public class Lecture
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان المحاضرة مطلوب")]
        [Display(Name = "عنوان المحاضرة")]
        public string Title { get; set; }

        [Display(Name = "وصف المحاضرة")]
        public string Description { get; set; }

        [Display(Name = "رابط الفيديو")]
        public string VideoUrl { get; set; }

        [Required(ErrorMessage = "تاريخ المحاضرة مطلوب")]
        [Display(Name = "تاريخ المحاضرة")]
        [DataType(DataType.Date)]
        public DateTime LectureDate { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}