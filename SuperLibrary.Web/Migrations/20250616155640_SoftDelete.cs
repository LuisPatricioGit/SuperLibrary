using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuperLibrary.Web.Migrations;

/// <inheritdoc />
public partial class SoftDelete : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "WasDeleted",
            table: "Books",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "WasDeleted",
            table: "Books");
    }
}