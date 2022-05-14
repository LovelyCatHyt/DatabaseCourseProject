using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using StudentManageSystem.Infos;

namespace StudentManageSystem.DataBase
{
    public abstract class DbValidator<T>
    {
        protected readonly DbContext _context;

        protected DbValidator(DbContext context)
        {
            _context = context;
        }
        
        public static bool NameValidate(string? name)
        {
            // 判空
            if (string.IsNullOrWhiteSpace(name)) return false;
            // 正则   
            return Regex.IsMatch(name, @"^[一-\uffff]+$");
        }

        public abstract InfoGroup ValidateRange(IEnumerable<T> dataEnumerable);

        public abstract InfoGroup ValidateRangeNew(IEnumerable<T> dataEnumerable);

    }
}
