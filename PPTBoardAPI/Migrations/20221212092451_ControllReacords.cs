using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPTBoardAPI.Migrations
{
    public partial class ControllReacords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ControllTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControllTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ControllRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    ControllTypeId = table.Column<int>(type: "int", nullable: false),
                    DisciplineId = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControllRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ControllRecords_ControllTypes_ControllTypeId",
                        column: x => x.ControllTypeId,
                        principalTable: "ControllTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ControllRecords_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ControllRecords_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ControllRecords_ControllTypeId",
                table: "ControllRecords",
                column: "ControllTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ControllRecords_DisciplineId",
                table: "ControllRecords",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_ControllRecords_StudentId",
                table: "ControllRecords",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ControllRecords");

            migrationBuilder.DropTable(
                name: "ControllTypes");
        }
    }
}
