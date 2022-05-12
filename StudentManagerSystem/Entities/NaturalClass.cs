using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudentManageSystem.DataBase;

namespace StudentManageSystem.Entities
{
    /// <summary>
    /// 自然班级
    /// </summary>
    public class NaturalClass: IDbVisitor
    {
        /// <summary>
        /// 班级号
        /// <para>主键</para>
        /// </summary>
        public int ClassId { get; set; }
        /// <summary>
        /// 学生
        /// <para>导航属性</para>
        /// </summary>
        public ICollection<Student> Students { get; set; } = new ObservableCollection<Student>();
        /// <summary>
        /// 院系号
        /// <para>外键</para>
        /// </summary>
        public int DepartmentId { get; set; }
        /// <summary>
        /// 院系名
        /// <para>UI 属性</para>
        /// </summary>
        [NotMapped]
        public string DepartmentName
        {
            get
            {
                return Department?.Name ?? "";
            }
            set
            {
                if (_context == null) return;
                Department = _context.Set<Department>().First(d=>d.Name == value);
                DepartmentId = Department.DepartmentId;
            }
        }
        /// <summary>
        /// 院系
        /// <para>导航属性</para>
        /// </summary>
        public Department? Department { get; set; }
        /// <summary>
        /// 专业名
        /// <para>外键</para>
        /// </summary>
        public string MajorName { get; set; } = "";
        /// <summary>
        /// 专业
        /// <para>导航属性</para>
        /// </summary>
        public Major? Major { get; set; }
        
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private DbContext? _context;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        public void Init(DbContext context) => _context = context;
    }
}
