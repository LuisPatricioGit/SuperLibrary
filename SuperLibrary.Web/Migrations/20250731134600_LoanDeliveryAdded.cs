using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuperLibrary.Web.Migrations
{
    /// <inheritdoc />
    public partial class LoanDeliveryAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanDetails_Books_BookId",
                table: "LoanDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanDetailsTemp_AspNetUsers_UserId",
                table: "LoanDetailsTemp");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanDetailsTemp_Books_BookId",
                table: "LoanDetailsTemp");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_UserId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "DaysOverdue",
                table: "LoanDetailsTemp");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "Loans",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "LoanDetails",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_LoanDetails_UserId",
                table: "LoanDetails",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanDetails_AspNetUsers_UserId",
                table: "LoanDetails",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanDetails_Books_BookId",
                table: "LoanDetails",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanDetailsTemp_AspNetUsers_UserId",
                table: "LoanDetailsTemp",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanDetailsTemp_Books_BookId",
                table: "LoanDetailsTemp",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_UserId",
                table: "Loans",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanDetails_AspNetUsers_UserId",
                table: "LoanDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanDetails_Books_BookId",
                table: "LoanDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanDetailsTemp_AspNetUsers_UserId",
                table: "LoanDetailsTemp");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanDetailsTemp_Books_BookId",
                table: "LoanDetailsTemp");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_UserId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_LoanDetails_UserId",
                table: "LoanDetails");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LoanDetails");

            migrationBuilder.AddColumn<int>(
                name: "DaysOverdue",
                table: "LoanDetailsTemp",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanDetails_Books_BookId",
                table: "LoanDetails",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanDetailsTemp_AspNetUsers_UserId",
                table: "LoanDetailsTemp",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanDetailsTemp_Books_BookId",
                table: "LoanDetailsTemp",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_UserId",
                table: "Loans",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
