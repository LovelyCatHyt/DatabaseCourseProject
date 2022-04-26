using System;

namespace StudentManageSystem.Entities
{
    /// <summary>
    /// 学生
    /// </summary>
    public class Student
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Birth { get; set; }
        public bool IsMale { get; set; }
        public int ClassId { get; set; }
        public NaturalClass Class { get; set; }
    }
}
