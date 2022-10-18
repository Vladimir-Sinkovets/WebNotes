using Microsoft.EntityFrameworkCore.Migrations;

namespace Notes.DAL.Migrations
{
    public partial class Add_tags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TagEntryId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoteEntryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Notes_NoteEntryId",
                        column: x => x.NoteEntryId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TagEntryId",
                table: "AspNetUsers",
                column: "TagEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_NoteEntryId",
                table: "Tags",
                column: "NoteEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Tags_TagEntryId",
                table: "AspNetUsers",
                column: "TagEntryId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Tags_TagEntryId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TagEntryId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TagEntryId",
                table: "AspNetUsers");
        }
    }
}
