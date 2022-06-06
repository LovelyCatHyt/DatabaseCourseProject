using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentManageSystem.DataBase;

namespace StudentManageSystem.Entities
{
    /// <summary>
    /// 选课
    /// </summary>
    public class CourseSelection : IDbVisitor
    {
        private DbContext? _context;

        public int Id { get; set; }
        public string StudentId { get; set; } = "";
        public Student Student { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }

        /// <summary>
        /// UI 属性: 选中的课程
        /// </summary>
        [NotMapped]
        public Course? SelectedCourse
        {
            get => Course;
            set
            {
                if (value == null) return;
                Course = value;
                CourseId = Course.Id;
            }
        }

        /// <summary>
        /// UI 属性: 可用的课程
        /// </summary>
        [NotMapped]
        public Course[] AvailableCourses => _context?.Set<Course>().ToArray() ?? new Course[0];

        public void Init(DbContext context)
        {
            _context = context;
        }
    }
}
