using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StudentManageSystem.Entities;

namespace StudentManageSystem.DataBase
{
    public class StudentDbValidator
    {
        public readonly DbContext studentDataBase;

        public StudentDbValidator(DbContext studentDataBase)
        {
            this.studentDataBase = studentDataBase;
        }

        public bool NameValidate(string? name)
        {
            // 判空
            if (string.IsNullOrWhiteSpace(name)) return false;
            // 特殊字符
            var inValidChars = "!@#$%^&*()_+-=<>[]{};:?'\",./\\\n\r\t".ToCharArray();
            if (inValidChars.Any(name.Contains)) return false;

            return true;
        }

        public bool ValidateData(Student student, out string msg)
        {
            var classSet = studentDataBase.Set<NaturalClass>();
            msg = "";

            // 外键约束
            if (!classSet.Any(c => c.ClassId == student.ClassId))
            {
                msg = $"不存在班级号 {student.ClassId}!";
                return false;
            }
            // 普通属性合理性约束
            if (!NameValidate(student.Name))
            {
                msg = $"姓名 \"{student.Name}\" 无效!";
                return false;
            }
            if (student.Birth < new DateTime(1904, 2, 11) || student.Birth > DateTime.Now)
            {
                // 当前在世最老长者: 露西尔·朗东, 生于1904年2月11日 https://zh.wikipedia.org/zh-cn/%E6%9C%80%E9%95%B7%E5%A3%BD%E8%80%85
                msg = $"出生日期 {student.Birth:D} 超出合理范围!";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 验证一个学生数据是否满足完整性约束, 可以插入到数据库中
        /// </summary>
        /// <param name="student">验证实体</param>
        /// <param name="msg">错误信息</param>
        /// <returns></returns>
        public bool ValidateNewData(Student student, out string msg)
        {
            var studentSet = studentDataBase.Set<Student>();
            msg = "";

            // 主键约束
            if (studentSet.Any(s => s.Id == student.Id))
            {
                msg = $"学号 {student.Id} 已被占用!";
                return false;
            }
            // 其它约束
            if (!ValidateData(student, out msg))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 验证一系列学生数据是否满足完整性约束, 可以批量插入到数据库中
        /// </summary>
        /// <param name="students"></param>
        /// <param name="msg">错误信息</param>
        /// <param name="stopOnFirstInvalid">检测到无效信息即停止</param>
        /// <returns></returns>
        public bool ValidateNewData(IEnumerable<Student> students, out string msg, bool stopOnFirstInvalid = false)
        {
            var strBuilder = new StringBuilder();
            bool allValid = true;
            var count = 0;
            var idSet = new HashSet<string>();
            foreach (var student in students)
            {
                if (!ValidateNewData(student, out var m))
                {
                    strBuilder.AppendLine($"[{count}] 学号为{student.Id}的数据异常: {m}");
                    allValid = false;
                    if (stopOnFirstInvalid) break;
                    count++;
                }
                else
                {
                    if (idSet.Contains(student.Id))
                    {
                        strBuilder.AppendLine($"[{count}] 学号为{student.Id}的数据异常: {student.Id} 已存在列表中");
                        allValid = false;
                        if (stopOnFirstInvalid) break;
                        count++;
                    }
                    else
                    {
                        idSet.Add(student.Id);
                    }
                }
                
            }

            msg = strBuilder.ToString();
            return allValid;
        }

        /// <summary>
        /// 验证一系列数据是否满足完整性约束, 且 IEnumerable 内数据的主键不冲突
        /// </summary>
        /// <param name="students"></param>
        /// <param name="msg"></param>
        /// <param name="stopOnFirstInvalid"></param>
        /// <returns></returns>
        public bool ValidateData(IEnumerable<Student> students, out string msg, bool stopOnFirstInvalid = false)
        {
            var strBuilder = new StringBuilder();
            bool allValid = true;
            var count = 0;
            var idSet = new HashSet<string>();
            foreach (var student in students)
            {
                if (!ValidateData(student, out var m))
                {
                    strBuilder.AppendLine($"[{count}] 学号为{student.Id}的数据异常: {m}");
                    allValid = false;
                    if (stopOnFirstInvalid) break;
                    count++;
                }
                else
                {
                    if (idSet.Contains(student.Id))
                    {
                        strBuilder.AppendLine($"[{count}] 学号为{student.Id}的数据异常: {student.Id} 已存在列表中");
                        allValid = false;
                        if (stopOnFirstInvalid) break;
                        count++;
                    }
                    else
                    {
                        idSet.Add(student.Id);
                    }
                }

            }

            msg = strBuilder.ToString();
            return allValid;
        }
    }
}
