using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StudentManageSystem.Entities
{
    /// <summary>
    /// 自然班级
    /// </summary>
    public class NaturalClass
    {
        public int ClassId { get; set; }
        public ICollection<Student> Students { get; set; } = new ObservableCollection<Student>();
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
