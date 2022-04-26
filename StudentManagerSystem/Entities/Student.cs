using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManageSystem.Entities
{
    /// <summary>
    /// 学生
    /// </summary>
    public class Student
    {
        public string Id { get; set; } = "";
        public string Name { get; set; }
        public DateTime Birth { get; set; }
        public bool IsMale { get; set; }
        [NotMapped]
        public string Gender
        {
            get => IsMale ? "男" : "女";
            set
            {
                switch (value.Trim())
                {
                    case "男":
                        IsMale = true;
                        break;
                    case "女":
                        IsMale = false;
                        break;
                }
            }
        }
        public int ClassId { get; set; }
        public NaturalClass Class { get; set; }

        public override bool Equals(object? obj)
        {
            if(obj is Student s)
            {
                return Equals(s);
            }

            return false;
        }

        protected bool Equals(Student other)
        {
            return Id == other.Id && Name == other.Name && Birth.Equals(other.Birth) && IsMale == other.IsMale && ClassId == other.ClassId;
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return HashCode.Combine(Id, Name, Birth, IsMale, ClassId);
        }
    }
}
