namespace StudentManageSystem.Entities
{
    /// <summary>
    /// 系
    /// </summary>
    public class Department
    {
        public int DepartmentId { get; set;}
        /// <summary>
        /// 院系名称
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// 院系类别
        /// </summary>
        public string DepartmentType { get; set; } = "";

        public override string ToString() => Name;
    }
}
