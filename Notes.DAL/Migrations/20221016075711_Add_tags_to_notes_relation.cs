using Microsoft.EntityFrameworkCore.Migrations;

namespace Notes.DAL.Migrations
{
    public partial class Add_tags_to_notes_relation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Tags_TagEntryId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Notes_NoteEntryId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_NoteEntryId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TagEntryId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NoteEntryId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TagEntryId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "NoteEntryTagEntry",
                columns: table => new
                {
                    NotesId = table.Column<int>(type: "int", nullable: false),
                    TagsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteEntryTagEntry", x => new { x.NotesId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_NoteEntryTagEntry_Notes_NotesId",
                        column: x => x.NotesId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NoteEntryTagEntry_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NoteEntryTagEntry_TagsId",
                table: "NoteEntryTagEntry",
                column: "TagsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NoteEntryTagEntry");

            migrationBuilder.AddColumn<int>(
                name: "NoteEntryId",
                table: "Tags",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TagEntryId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_NoteEntryId",
                table: "Tags",
                column: "NoteEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TagEntryId",
                table: "AspNetUsers",
                column: "TagEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Tags_TagEntryId",
                table: "AspNetUsers",
                column: "TagEntryId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Notes_NoteEntryId",
                table: "Tags",
                column: "NoteEntryId",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
