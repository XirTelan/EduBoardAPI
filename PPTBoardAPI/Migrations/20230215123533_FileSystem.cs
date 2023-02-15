using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPTBoardAPI.Migrations
{
    public partial class FileSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileSystemObjs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SystemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFolder = table.Column<bool>(type: "bit", nullable: false),
                    ParentFolderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileSystemObjs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileSystemObjs_FileSystemObjs_ParentFolderId",
                        column: x => x.ParentFolderId,
                        principalTable: "FileSystemObjs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileSystemObjs_ParentFolderId",
                table: "FileSystemObjs",
                column: "ParentFolderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileSystemObjs");
        }
    }
}
