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
    public class MajorDbValidator : DbValidator<Major>
    {
        private readonly DbSet<Major> _majors;

        public MajorDbValidator(DbContext context) : base(context)
        {
            _majors = context.Set<Major>();
        }

        public InfoGroup ValidateData(Major m)
        {
            var info = new InfoGroup();

            // TODO: 专业的外键约束
            // 普通属性约束
            if (!NameValidate(m.MajorName))
            {
                info.AddOrConcat(new Info(m, $"专业{{{m.DepartmentName}, {m.MajorName}}}: 专业名\"{m.MajorName}\"无效!"));
            }

            return info;
        }

        public InfoGroup ValidateNewData(Major major)
        {
            var info = new InfoGroup();

            if (_majors.Any(m => m.DepartmentId == major.DepartmentId && m.MajorName == major.MajorName))
            {
                info.AddOrConcat(new Error(major, $"专业{{{major.DepartmentName}, {major.MajorName}}}: 该专业已存在!"));
            }

            info.AddOrConcat(ValidateData(major));

            return info;
        }

        public override InfoGroup ValidateRange(IEnumerable<Major> dataEnumerable) =>
            dataEnumerable.Aggregate(new InfoGroup(), (i, m) =>
            {
                i.AddOrConcat(ValidateData(m));
                return i;
            });

        public override InfoGroup ValidateRangeNew(IEnumerable<Major> dataEnumerable)
        {
            var set = new HashSet<(int, string)>();
            var info = new InfoGroup();
            foreach (var major in dataEnumerable)
            {
                var temp = (major.DepartmentId, major.MajorName);
                if (set.Contains(temp))
                {
                    info.AddOrConcat(new Error(major, $"专业{{{major.DepartmentName}, {major.MajorName}}}: 该专业已存在!"));
                }
                else
                {
                    set.Add(temp);
                }

                info.AddOrConcat(ValidateNewData(major));
            }

            return info;
        }
    }
}
