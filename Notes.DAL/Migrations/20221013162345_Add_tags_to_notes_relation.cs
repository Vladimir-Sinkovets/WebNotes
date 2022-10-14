using Microsoft.EntityFrameworkCore.Migrations;

namespace Notes.DAL.Migrations
{
    public partial class Add_tags_to_notes_relation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TagEntryId",
                table: "Tags",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagEntryId",
                table: "Tags",
                column: "TagEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Tags_TagEntryId",
                table: "Tags",
                column: "TagEntryId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Tags_TagEntryId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TagEntryId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TagEntryId",
                table: "Tags");
        }
    }
}
