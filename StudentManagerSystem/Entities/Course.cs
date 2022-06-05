using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManageSystem.Entities
{
    /// <summary>
    /// 课程
    /// </summary>
    public class Course
    {
        /// <summary>
        /// 课程编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 课程名
        /// </summary>
        public string Name { get; set; } = "";
    }
}
