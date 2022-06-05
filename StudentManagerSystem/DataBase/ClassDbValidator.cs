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
    public class ClassDbValidator : DbValidator<NaturalClass, int, int>
    {
        public ClassDbValidator(DbContext context) : base(context) { }

        public override int IdOfEntity(NaturalClass entity) => entity.ClassId;
        public override int NameOfEntity(NaturalClass entity) => entity.ClassId;
        
        public override InfoGroup ValidateData(NaturalClass cls)
        {
            var info = new InfoGroup();

            // TODO: 班级的外键约束
            // 班级目前没有普通属性的约束
            return info;
        }

        public override InfoGroup ValidateRangeNew(IEnumerable<NaturalClass> dataEnumerable) =>
            ValidateRangeNew(dataEnumerable, "班级{0}: 班级号 {1} 已被占用!");
    }
}
