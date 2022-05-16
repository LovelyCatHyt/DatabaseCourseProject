using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace StudentManageSystem
{
    public static class SqlViewer
    {
        public static string[] SqlStatements => _sqlSet.Select(str => str.Replace("\"", "")).ToArray();
        public static event Action? OnSqlUpdated;

        private static readonly HashSet<string> _sqlSet = new();

        public static void Add(string sql)
        {
            _sqlSet.Add(sql);
            SqlUpdated();
        }

        public static void Add(IQueryable queryable) => Add(queryable.ToQueryString());

        public static void Remove(IQueryable queryable) => Remove(queryable.ToQueryString());

        public static void Remove(string sql)
        {
            _sqlSet.Remove(sql);
            SqlUpdated();
        }

        public static void Clear()
        {
            _sqlSet.Clear();
            SqlUpdated();
        }

        private static void SqlUpdated()
        {
            OnSqlUpdated?.Invoke();
        }
    }
}
