using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentManageSystem.Entities;
using StudentManageSystem.Infos;

namespace StudentManageSystem.DataBase
{
    public class CourseSelectionDbValidator : DbValidator<CourseSelection, int, string>
    {
        private readonly DbSet<Student> _students;
        private readonly DbSet<Course> _courses;

        public CourseSelectionDbValidator(DbContext context) : base(context)
        {
            _students = context.Set<Student>();
            _courses = context.Set<Course>();
        }
        public override int IdOfEntity(CourseSelection entity) => entity.Id;

        public override string NameOfEntity(CourseSelection entity) =>
            $"{entity.Student.Name}(ѧ��: {entity.StudentId})��ѡ��{entity.CourseId}";

        public override InfoGroup ValidateData(CourseSelection data)
        {
            var info = new InfoGroup();
            // ������
            if (!_students.Any(s => s.Id == data.StudentId))
            {
                info.AddOrConcat(new Info($"{NameOfEntity(data)}: ѧ��{data.StudentId}������!"));
            }
            if (!_courses.Any(c => c.Id == data.CourseId))
            {
                info.AddOrConcat(new Info($"{NameOfEntity(data)}: �γ̺�{data.CourseId}������!"));
            }

            return info;
        }

        public override InfoGroup ValidateRangeNew(IEnumerable<CourseSelection> dataEnumerable) =>
            ValidateRangeNew(dataEnumerable, "{0}: ѡ��Id{1}�ѱ�ռ��!");

    }
}
