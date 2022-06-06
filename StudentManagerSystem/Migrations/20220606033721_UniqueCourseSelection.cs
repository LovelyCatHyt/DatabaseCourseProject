using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentManageSystem.Migrations
{
    public partial class UniqueCourseSelection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Selections_StudentId",
                table: "Selections");

            migrationBuilder.CreateIndex(
                name: "IX_Selections_CourseId",
                table: "Selections",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Selections_StudentId_CourseId",
                table: "Selections",
                columns: new[] { "StudentId", "CourseId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Selections_Courses_CourseId",
                table: "Selections",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Selections_Courses_CourseId",
                table: "Selections");

            migrationBuilder.DropIndex(
                name: "IX_Selections_CourseId",
                table: "Selections");

            migrationBuilder.DropIndex(
                name: "IX_Selections_StudentId_CourseId",
                table: "Selections");

            migrationBuilder.CreateIndex(
                name: "IX_Selections_StudentId",
                table: "Selections",
                column: "StudentId");
        }
    }
}
