using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Notes.DAL.Migrations
{
    public partial class Add_CreatedDate_column_for_note : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Notes",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Notes");
        }
    }
}
