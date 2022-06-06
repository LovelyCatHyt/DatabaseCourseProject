using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentManageSystem.DataBase;
using StudentManageSystem.Entities;

namespace StudentManageSystem.ViewModels
{
    public class StudentDetail
    {
        private readonly DbContext _context;
        private readonly DbSet<CourseSelection> _courseSelections;

        public Student target;

        public string Id => target.Id;

        public string Name
        {
            get => target.Name;
            set => target.Name = value;
        }

        public DateTime Birth => target.Birth;

        public string Gender
        {
            get => target.Gender;
            set => target.Gender = value;
        }

        public Major Major => Class.Major;

        public Department Department => Class.Department;

        public CourseSelection[] CourseSelections => 
            _courseSelections.Where(cs => cs.StudentId == Id).ToArray();

        public NaturalClass Class => target.Class;
        
        public StudentDetail(Student target, DbContext context)
        {
            this.target = target;
            _context = context;
            _courseSelections = _context.Set<CourseSelection>();
        }
    }
}
