using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemenBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaFieldsToPropertyImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MediaType",
                table: "PropertyImages",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "image",
                comment: "نوع الوسائط image/video");

            migrationBuilder.AddColumn<int>(
                name: "DurationSeconds",
                table: "PropertyImages",
                type: "int",
                nullable: true,
                comment: "مدة الفيديو بالثواني");

            migrationBuilder.AddColumn<string>(
                name: "VideoThumbnailUrl",
                table: "PropertyImages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "رابط المصغرة للفيديو");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "PropertyImages");

            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "PropertyImages");

            migrationBuilder.DropColumn(
                name: "VideoThumbnailUrl",
                table: "PropertyImages");
        }
    }
}

