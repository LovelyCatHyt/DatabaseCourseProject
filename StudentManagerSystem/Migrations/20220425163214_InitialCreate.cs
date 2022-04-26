using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentManageSystem.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    DepartmentType = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    ClassId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DepartmentId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.ClassId);
                    table.ForeignKey(
                        name: "FK_Classes_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Birth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsMale = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClassId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentId", "DepartmentType", "Name" },
                values: new object[] { 1, "工科", "自动化学院" });

            migrationBuilder.InsertData(
                table: "Classes",
                columns: new[] { "ClassId", "DepartmentId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Birth", "ClassId", "IsMale", "Name" },
                values: new object[] { "2022000001", new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, "张三" });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Birth", "ClassId", "IsMale", "Name" },
                values: new object[] { "2022000002", new DateTime(2001, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "李四" });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Birth", "ClassId", "IsMale", "Name" },
                values: new object[] { "2022000003", new DateTime(2001, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, "王五" });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_DepartmentId",
                table: "Classes",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ClassId",
                table: "Students",
                column: "ClassId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
