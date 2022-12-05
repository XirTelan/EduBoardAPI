using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPTBoardAPI.Migrations
{
    public partial class GroupUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Specialities_SpecialityId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "CuratorId",
                table: "Groups");

            migrationBuilder.AlterColumn<int>(
                name: "SpecialityId",
                table: "Groups",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "PersonId",
                table: "Groups",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_PersonId",
                table: "Groups",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_AspNetUsers_PersonId",
                table: "Groups",
                column: "PersonId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Specialities_SpecialityId",
                table: "Groups",
                column: "SpecialityId",
                principalTable: "Specialities",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_AspNetUsers_PersonId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Specialities_SpecialityId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_PersonId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Groups");

            migrationBuilder.AlterColumn<int>(
                name: "SpecialityId",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CuratorId",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Specialities_SpecialityId",
                table: "Groups",
                column: "SpecialityId",
                principalTable: "Specialities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
