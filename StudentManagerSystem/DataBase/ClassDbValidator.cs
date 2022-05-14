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
    public class ClassDbValidator : DbValidator<NaturalClass>
    {
        private readonly DbSet<NaturalClass> _classes;

        public ClassDbValidator(DbContext context) : base(context)
        {
            _classes = context.Set<NaturalClass>();
        }

        public InfoGroup ValidateData(NaturalClass cls)
        {
            var info = new InfoGroup();

            // TODO: 班级的外键约束
            // 班级目前没有普通属性的约束
            return info;
        }

        public InfoGroup ValidateNewData(NaturalClass cls)
        {
            var info = new InfoGroup();
            if (_classes.Any(c => c.ClassId == cls.ClassId))
            {
                info.AddOrConcat(new Error(cls, $"班级{cls.ClassId}: 班级号 {cls.ClassId} 已被使用!"));
            }

            info.AddOrConcat(ValidateData(cls));

            return info;
        }

        public override InfoGroup ValidateRange(IEnumerable<NaturalClass> dataEnumerable) =>
            dataEnumerable.Aggregate(new InfoGroup(), (i, c) =>
            {
                i.AddOrConcat(ValidateData(c));
                return i;
            });

        public override InfoGroup ValidateRangeNew(IEnumerable<NaturalClass> dataEnumerable)
        {
            var set = new HashSet<int>();
            var info = new InfoGroup();
            foreach (var naturalClass in dataEnumerable)
            {
                if (set.Contains(naturalClass.ClassId))
                {
                    info.AddOrConcat(new Error(naturalClass, $"班级{naturalClass.ClassId}: 班级号 {naturalClass.ClassId} 已被占用!"));
                }
                else
                {
                    set.Add(naturalClass.ClassId);
                }

                info.AddOrConcat(ValidateNewData(naturalClass));
            }

            return info;
        }
    }
}
