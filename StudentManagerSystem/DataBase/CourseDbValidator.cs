using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudentManageSystem.Entities;
using StudentManageSystem.Infos;

namespace StudentManageSystem.DataBase
{
    public class CourseDbValidator : DbValidator<Course, int, string>
    {
        public CourseDbValidator(DbContext context) : base(context) { }
        public override int IdOfEntity(Course entity) => entity.Id;
        public override string NameOfEntity(Course entity) => entity.Name;

        public override InfoGroup ValidateData(Course course)
        {
            var info = new InfoGroup();
            if (!NameValidate(course.Name))
            {
                info.AddOrConcat(new Info(course, $"课程{course.Name}: 课程名\"{course.Name}\"无效!"));
            }
            return info;
        }

        public override InfoGroup ValidateRangeNew(IEnumerable<Course> dataEnumerable) =>
            ValidateRangeNew(dataEnumerable, "课程{0}: 课程编号{1}已被占用!");
    }
}
