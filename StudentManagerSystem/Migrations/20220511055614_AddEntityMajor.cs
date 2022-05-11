using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentManageSystem.Migrations
{
    public partial class AddEntityMajor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Classes_DepartmentId",
                table: "Classes");

            migrationBuilder.AddColumn<string>(
                name: "MajorName",
                table: "Classes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Majors",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    MajorName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Majors", x => new { x.DepartmentId, x.MajorName });
                    table.ForeignKey(
                        name: "FK_Majors_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Majors",
                columns: new[] { "DepartmentId", "MajorName" },
                values: new object[] { 1, "自动化与电气类" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "ClassId",
                keyValue: 1,
                column: "MajorName",
                value: "自动化与电气类");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_DepartmentId_MajorName",
                table: "Classes",
                columns: new[] { "DepartmentId", "MajorName" });

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Majors_DepartmentId_MajorName",
                table: "Classes",
                columns: new[] { "DepartmentId", "MajorName" },
                principalTable: "Majors",
                principalColumns: new[] { "DepartmentId", "MajorName" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Majors_DepartmentId_MajorName",
                table: "Classes");

            migrationBuilder.DropTable(
                name: "Majors");

            migrationBuilder.DropIndex(
                name: "IX_Classes_DepartmentId_MajorName",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "MajorName",
                table: "Classes");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_DepartmentId",
                table: "Classes",
                column: "DepartmentId");
        }
    }
}
