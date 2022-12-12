using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPTBoardAPI.Migrations
{
    public partial class ControllReacordsFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudId",
                table: "ControllRecords");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudId",
                table: "ControllRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
