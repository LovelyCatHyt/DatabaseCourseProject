﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StudentManageSystem.DataBase;

namespace StudentManageSystem.Migrations
{
    [DbContext(typeof(StudentDataBase))]
    partial class StudentDataBaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.16");

            modelBuilder.Entity("StudentManageSystem.Entity.Department", b =>
                {
                    b.Property<int>("DepartmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DepartmentType")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("DepartmentId");

                    b.ToTable("Departments");

                    b.HasData(
                        new
                        {
                            DepartmentId = 1,
                            DepartmentType = "工科",
                            Name = "自动化学院"
                        });
                });

            modelBuilder.Entity("StudentManageSystem.Entity.NaturalClass", b =>
                {
                    b.Property<int>("ClassId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ClassId");

                    b.HasIndex("DepartmentId");

                    b.ToTable("Classes");

                    b.HasData(
                        new
                        {
                            ClassId = 1,
                            DepartmentId = 1
                        });
                });

            modelBuilder.Entity("StudentManageSystem.Entity.Student", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Birth")
                        .HasColumnType("TEXT");

                    b.Property<int>("ClassId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsMale")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ClassId");

                    b.ToTable("Students");

                    b.HasData(
                        new
                        {
                            Id = "2022000001",
                            Birth = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ClassId = 1,
                            IsMale = true,
                            Name = "张三"
                        },
                        new
                        {
                            Id = "2022000002",
                            Birth = new DateTime(2001, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ClassId = 1,
                            IsMale = false,
                            Name = "李四"
                        },
                        new
                        {
                            Id = "2022000003",
                            Birth = new DateTime(2001, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ClassId = 1,
                            IsMale = true,
                            Name = "王五"
                        });
                });

            modelBuilder.Entity("StudentManageSystem.Entity.NaturalClass", b =>
                {
                    b.HasOne("StudentManageSystem.Entity.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");
                });

            modelBuilder.Entity("StudentManageSystem.Entity.Student", b =>
                {
                    b.HasOne("StudentManageSystem.Entity.NaturalClass", "Class")
                        .WithMany("Students")
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Class");
                });

            modelBuilder.Entity("StudentManageSystem.Entity.NaturalClass", b =>
                {
                    b.Navigation("Students");
                });
#pragma warning restore 612, 618
        }
    }
}
