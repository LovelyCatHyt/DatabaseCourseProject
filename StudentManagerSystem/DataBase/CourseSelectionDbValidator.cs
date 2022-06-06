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
            $"{entity.Student.Name}(学号: {entity.StudentId})的选课{entity.CourseId}";

        public override InfoGroup ValidateData(CourseSelection data)
        {
            var info = new InfoGroup();
            // 检查外键
            if (!_students.Any(s => s.Id == data.StudentId))
            {
                info.AddOrConcat(new Info($"{NameOfEntity(data)}: 学号{data.StudentId}不存在!"));
            }
            if (!_courses.Any(c => c.Id == data.CourseId))
            {
                info.AddOrConcat(new Info($"{NameOfEntity(data)}: 课程号{data.CourseId}不存在!"));
            }

            return info;
        }

        protected override InfoGroup ValidateNewData(CourseSelection data, string idDuplicatedErrorMsg)
        {
            var info = base.ValidateNewData(data, idDuplicatedErrorMsg);
            if (_entities.Any(e => e.StudentId == data.StudentId && e.CourseId == data.CourseId))
            {
                info.AddOrConcat(new Error($"选课 {NameOfEntity(data)}: 选课重复!"));
            }
            return info;
        }

        public override InfoGroup ValidateRangeNew(IEnumerable<CourseSelection> dataEnumerable) =>
            ValidateRangeNew(dataEnumerable, "{0}: 选课Id{1}已被占用!");

    }
}
