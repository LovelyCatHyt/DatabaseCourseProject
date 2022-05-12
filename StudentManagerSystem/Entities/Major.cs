using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudentManageSystem.DataBase;

namespace StudentManageSystem.Entities
{
    /// <summary>
    /// 专业
    /// </summary>
    public class Major: IDbVisitor
    {
        /// <summary>
        /// 院系号
        /// <para>主键 Part1</para>
        /// </summary>
        public int DepartmentId { get; set; }
        /// <summary>
        /// 专业名
        /// <para>主键 Part2</para>
        /// </summary>
        public string MajorName { get; set; } = "";
        /// <summary>
        /// 院系名
        /// <para>UI 属性</para>
        /// </summary>
        [NotMapped]
        public string DepartmentName
        {
            get
            {
                // Debug.WriteLine($"get {MajorName}.DepartmentName when (_context == null) is {_context == null}");
                if (_context == null) return "";
                return _context.Set<Department>().First(d=>d.DepartmentId == DepartmentId).Name;
            }
            set
            {
                if (_context == null) return;
                var oldDepartmentId = DepartmentId;
                DepartmentId = _context.Set<Department>().First(d => d.Name == value).DepartmentId;
                var dependents = _context.Set<NaturalClass>()
                    .Where(c => c.DepartmentId == oldDepartmentId && c.MajorName == MajorName).ToArray();
                foreach (var c in dependents)
                {
                    _context.Remove(c);
                    c.DepartmentId = DepartmentId;
                    _context.Add(c);
                }

                _context.SaveChanges();
                // _context.UpdateRange(dependents);
            }
        }
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private DbContext? _context;

        public void Init(DbContext ctx) => _context = ctx;
    }
}
