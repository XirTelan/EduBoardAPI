using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPTBoardAPI.Migrations
{
    public partial class ChangeKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SpecialityId",
                table: "Specialities",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Groups",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "DisciplineId",
                table: "Disciplines",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Specialities",
                newName: "SpecialityId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Groups",
                newName: "GroupId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Disciplines",
                newName: "DisciplineId");
        }
    }
}
