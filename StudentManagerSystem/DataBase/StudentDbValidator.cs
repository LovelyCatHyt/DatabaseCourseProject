using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using StudentManageSystem.Entities;
using StudentManageSystem.Infos;

namespace StudentManageSystem.DataBase
{
    public class StudentDbValidator : DbValidator<Student, string, string>
    {
        public StudentDbValidator(DbContext context) : base(context) { }
        
        public override string IdOfEntity(Student entity) => entity.Id;

        public override string NameOfEntity(Student entity) => $"{entity.Name} ({entity.Id})";

        public override InfoGroup ValidateData(Student data)
        {
            var classSet = _context.Set<NaturalClass>();
            var infoGroup = new InfoGroup();

            // 外键约束
            if (!classSet.Any(c => c.ClassId == data.ClassId))
            {
                infoGroup.AddOrConcat(new Error(data, $"学生{data.Id}: 不存在班级号 {data.ClassId}!"));
            }
            // 普通属性合理性约束
            if (!NameValidate(data.Name))
            {
                infoGroup.AddOrConcat(new Info(data, $"学生{data.Id}: 姓名 \"{data.Name}\" 无效!"));
            }
            if (data.Birth < new DateTime(1904, 2, 11) || data.Birth > DateTime.Now)
            {
                // 当前在世最老长者: 露西尔·朗东, 生于1904年2月11日 https://zh.wikipedia.org/zh-cn/%E6%9C%80%E9%95%B7%E5%A3%BD%E8%80%85
                infoGroup.AddOrConcat(new Info(data, $"学生{data.Id}: 出生日期 {data.Birth:D} 超出合理范围!"));
            }

            return infoGroup;
        }
        
        public override InfoGroup ValidateRangeNew(IEnumerable<Student> dataEnumerable)
            => ValidateRangeNew(dataEnumerable, "学生 {0}: 学号 {1} 已存在列表中");
    }
}
