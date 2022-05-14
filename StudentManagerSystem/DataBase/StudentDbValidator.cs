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
    public class StudentDbValidator : DbValidator<Student>
    {
        public StudentDbValidator(DbContext context) : base(context) { }

        public static bool NameValidate(string? name)
        {
            // 判空
            if (string.IsNullOrWhiteSpace(name)) return false;
            // 正则   
            return Regex.IsMatch(name, @"^[一-\uffff]+$");
        }

        public InfoGroup ValidateData(Student student, out string msg)
        {
            var classSet = _context.Set<NaturalClass>();
            var infoGroup = new InfoGroup();
            
            // 外键约束
            if (!classSet.Any(c => c.ClassId == student.ClassId))
            {
                infoGroup.AddOrConcat(new Error(student, $"学生{student.Id}: 不存在班级号 {student.ClassId}!"));
            }
            // 普通属性合理性约束
            if (!NameValidate(student.Name))
            {
                infoGroup.AddOrConcat(new Info(student, $"学生{student.Id}: 姓名 \"{student.Name}\" 无效!"));
            }
            if (student.Birth < new DateTime(1904, 2, 11) || student.Birth > DateTime.Now)
            {
                // 当前在世最老长者: 露西尔·朗东, 生于1904年2月11日 https://zh.wikipedia.org/zh-cn/%E6%9C%80%E9%95%B7%E5%A3%BD%E8%80%85
                infoGroup.AddOrConcat(new Info(student, $"学生{student.Id}: 出生日期 {student.Birth:D} 超出合理范围!"));
            }

            msg = infoGroup;
            return infoGroup;
        }

        /// <summary>
        /// 验证一个学生数据是否满足完整性约束, 可以插入到数据库中
        /// </summary>
        /// <param name="student">验证实体</param>
        /// <param name="msg">错误信息</param>
        /// <returns></returns>
        public InfoGroup ValidateNewData(Student student, out string msg)
        {
            var studentSet = _context.Set<Student>();
            var infoGroup = new InfoGroup();

            // 主键约束
            if (studentSet.Any(s => s.Id == student.Id))
            {
                infoGroup.AddOrConcat(new Error(student, $"学生{student.Id}: 学号 {student.Id} 已被占用!"));
            }
            // 其它约束
            infoGroup.AddOrConcat(ValidateData(student, out _));

            msg = infoGroup;
            return infoGroup;
        }

        private InfoGroup ValidateEnumerable(IEnumerable<Student> students, out string msg, bool stopOnFirstInvalid, Func<Student, BaseInfo> elementValidator)
        {
            var infoGroup = new InfoGroup();
            var idSet = new HashSet<string>();
            foreach (var student in students)
            {
                var info = elementValidator(student);
                infoGroup.AddOrConcat(info);
                BaseInfo? noDuplicate = null;
                // 查重
                if (idSet.Contains(student.Id))
                {
                    infoGroup.AddOrConcat(noDuplicate = new Error(student, $"学生{student.Id}: 学号 {student.Id} 已存在列表中"));
                }
                else
                {
                    idSet.Add(student.Id);
                }
                // 中断
                if (!info || (noDuplicate != null && !noDuplicate))
                {
                    if (stopOnFirstInvalid) break;
                }
            }

            msg = infoGroup;
            return infoGroup;
        }

        /// <summary>
        /// 验证一系列学生数据是否满足完整性约束, 可以批量插入到数据库中
        /// </summary>
        /// <param name="students"></param>
        /// <param name="msg">错误信息</param>
        /// <param name="stopOnFirstInvalid">检测到无效信息即停止</param>
        /// <returns></returns>
        public InfoGroup ValidateNewData(IEnumerable<Student> students, out string msg, bool stopOnFirstInvalid = false)
        {
            return ValidateEnumerable(students, out msg, stopOnFirstInvalid, s => ValidateNewData(s, out _));
        }

        /// <summary>
        /// 验证一系列数据是否满足完整性约束, 且 IEnumerable 内数据的主键不冲突
        /// </summary>
        /// <param name="students"></param>
        /// <param name="msg"></param>
        /// <param name="stopOnFirstInvalid"></param>
        /// <returns></returns>
        public InfoGroup ValidateData(IEnumerable<Student> students, out string msg, bool stopOnFirstInvalid = false)
        {
            return ValidateEnumerable(students, out msg, stopOnFirstInvalid, s => ValidateData(s, out _));
        }


        public override InfoGroup ValidateRange(IEnumerable<Student> dataEnumerable) 
            => ValidateData(dataEnumerable, out _);

        public override InfoGroup ValidateRangeNew(IEnumerable<Student> dataEnumerable)
            => ValidateNewData(dataEnumerable, out _);
    }
}
