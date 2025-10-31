using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuperLibrary.Web.Migrations
{
    /// <inheritdoc />
    public partial class AdminConfirm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdminConfirmed",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdminConfirmed",
                table: "AspNetUsers");
        }
    }
}
