using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManageSystem.Entities
{
    /// <summary>
    /// 专业
    /// </summary>
    public class Major
    {
        public int DepartmentId { get; set; }
        public string MajorName { get; set; } = "";
        // public Department? Department { get; set; }
    }
}
