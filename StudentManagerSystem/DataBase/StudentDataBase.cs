using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StudentManageSystem.Entities;

namespace StudentManageSystem.DataBase
{
    public class StudentDataBase : DbContext
    {
        public const string DefaultDbPath = "data\\student.db";

        public string DbPath { get; }

        public DbSet<Student> Students { get; set; }
        public DbSet<NaturalClass> Classes { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Major> Majors { get; set; }

        public StudentDataBase()
        {
            // 就在当前路径, 但 GetFullPath 可以解决一切斜杠不统一和夹杂相对路径的问题
            DbPath = Path.GetFullPath(Path.Join(Directory.GetCurrentDirectory(), DefaultDbPath));
            var dir = Path.GetDirectoryName(DbPath);
            if (dir != null) Directory.CreateDirectory(dir);
            // Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = new SqliteConnectionStringBuilder
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
                DataSource = DbPath
            }.ToString();
            optionsBuilder.UseSqlite(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 主键
            modelBuilder.Entity<Student>()
                .HasKey(s => s.Id);
            modelBuilder.Entity<NaturalClass>()
                .HasKey(c => c.ClassId);
            modelBuilder.Entity<Department>()
                .HasKey(dep => dep.DepartmentId);
            modelBuilder.Entity<Major>()
                .HasKey(m => new { m.DepartmentId, m.MajorName });

            // 外键
            modelBuilder.Entity<NaturalClass>()
                .HasOne(c => c.Department)
                .WithMany()
                .HasForeignKey(c => c.DepartmentId);
            modelBuilder.Entity<Student>()
                .HasOne(c => c.Class)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.ClassId);
            modelBuilder.Entity<Major>()
                .HasOne<Department>()
                .WithMany()
                .HasForeignKey(m => m.DepartmentId);
            modelBuilder.Entity<NaturalClass>()
                .HasOne<Major>(c => c.Major)
                .WithMany()
                .HasForeignKey(c => new { c.DepartmentId, c.MajorName });

            // 索引
            modelBuilder.Entity<Department>()
                .HasIndex(d => d.Name)
                .IsUnique();

            // 种子数据, 但不包含直接的引用
            // 一个问题: 为啥 Id=0 会抛异常?
            var testDepartment = new Department
            {
                DepartmentId = 1,
                DepartmentType = "工科",
                Name = "自动化学院"
            };
            var testMajor = new Major
            {
                DepartmentId = 1,
                MajorName = "自动化与电气类"
            };
            var testClass = new NaturalClass
            {
                DepartmentId = testDepartment.DepartmentId,
                // Department = testDepartment,
                ClassId = 1,
                MajorName = testMajor.MajorName
                // Students = new List<Student>()
            };

            modelBuilder.Entity<Department>()
                .HasData(testDepartment);
            modelBuilder.Entity<Major>()
                .HasData(testMajor);
            modelBuilder.Entity<NaturalClass>()
                .HasData(testClass);
            modelBuilder.Entity<Student>()
                .HasData(
                    new Student
                    {
                        Birth = new DateTime(2001, 1, 1),
                        ClassId = testClass.ClassId,
                        Id = "2022000001",
                        IsMale = true,
                        Name = "张三"

                    },
                    new Student
                    {
                        Birth = new DateTime(2001, 2, 3),
                        ClassId = testClass.ClassId,
                        Id = "2022000002",
                        IsMale = false,
                        Name = "李四"

                    },
                    new Student
                    {
                        Birth = new DateTime(2001, 5, 8),
                        ClassId = testClass.ClassId,
                        Id = "2022000003",
                        IsMale = true,
                        Name = "王五"
                    }
                );
        }
    }
}
