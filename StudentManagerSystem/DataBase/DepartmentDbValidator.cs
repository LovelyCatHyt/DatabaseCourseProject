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
    public class DepartmentDbValidator: DbValidator<Department, int, string>
    {
        private readonly DbSet<Department> _departments;

        public DepartmentDbValidator(DbContext context) : base(context) => _departments = context.Set<Department>();

        public override int IdOfEntity(Department entity) => entity.DepartmentId;

        public override string NameOfEntity(Department entity) => entity.Name;

        public override InfoGroup ValidateData(Department department)
        {
            var info = new InfoGroup();

            // TODO: 院系的外键约束
            // 普通属性约束
            if (!NameValidate(department.Name))
            {
                info.AddOrConcat(new Info(department, $"院系{department.Name}: 院系名\"{department.Name}\"无效!"));
            }

            return info;
        }

        public InfoGroup ValidateNewData(Department department)
        {
            var info = new InfoGroup();

            if (_departments.Any(m => m.DepartmentId == department.DepartmentId))
            {
                info.AddOrConcat(new Error(department, $"院系{department.Name}: 院系号 {department.DepartmentId} 已被占用!"));
            }

            info.AddOrConcat(ValidateData(department));

            return info;
        }

        public override InfoGroup ValidateRange(IEnumerable<Department> dataEnumerable) =>
            dataEnumerable.Aggregate(new InfoGroup(), (i, d) =>
            {
                i.AddOrConcat(ValidateData(d));
                return i;
            });


        public override InfoGroup ValidateRangeNew(IEnumerable<Department> dataEnumerable)
        {
            var set = new HashSet<int>();
            var info = new InfoGroup();
            foreach (var naturalClass in dataEnumerable)
            {
                if (set.Contains(naturalClass.DepartmentId))
                {
                    info.AddOrConcat(new Error(naturalClass, $"院系{naturalClass.DepartmentId}: 院系号 {naturalClass.DepartmentId} 已被占用!"));
                }
                else
                {
                    set.Add(naturalClass.DepartmentId);
                }

                info.AddOrConcat(ValidateNewData(naturalClass));
            }

            return info;
        }

    }
}
