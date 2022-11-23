using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPTBoardAPI.Migrations
{
    public partial class AddDisciplines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecialityDiscipline_Discipline_DisciplineId",
                table: "SpecialityDiscipline");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Discipline",
                table: "Discipline");

            migrationBuilder.RenameTable(
                name: "Discipline",
                newName: "Disciplines");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Disciplines",
                table: "Disciplines",
                column: "DisciplineId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecialityDiscipline_Disciplines_DisciplineId",
                table: "SpecialityDiscipline",
                column: "DisciplineId",
                principalTable: "Disciplines",
                principalColumn: "DisciplineId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecialityDiscipline_Disciplines_DisciplineId",
                table: "SpecialityDiscipline");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Disciplines",
                table: "Disciplines");

            migrationBuilder.RenameTable(
                name: "Disciplines",
                newName: "Discipline");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Discipline",
                table: "Discipline",
                column: "DisciplineId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecialityDiscipline_Discipline_DisciplineId",
                table: "SpecialityDiscipline",
                column: "DisciplineId",
                principalTable: "Discipline",
                principalColumn: "DisciplineId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
