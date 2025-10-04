using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemenBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitCancellationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowsCancellation",
                table: "Units",
                type: "bit",
                nullable: false,
                defaultValue: true,
                comment: "هل تقبل الوحدة إلغاء الحجز");

            migrationBuilder.AddColumn<int>(
                name: "CancellationWindowDays",
                table: "Units",
                type: "int",
                nullable: true,
                comment: "عدد أيام نافذة الإلغاء قبل الوصول");

            migrationBuilder.CreateIndex(
                name: "IX_Units_AllowsCancellation",
                table: "Units",
                column: "AllowsCancellation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Units_AllowsCancellation",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "AllowsCancellation",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "CancellationWindowDays",
                table: "Units");
        }
    }
}

