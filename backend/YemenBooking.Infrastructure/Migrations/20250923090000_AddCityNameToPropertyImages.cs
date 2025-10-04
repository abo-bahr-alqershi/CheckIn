using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemenBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCityNameToPropertyImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add CityName column to PropertyImages
            migrationBuilder.AddColumn<string>(
                name: "CityName",
                table: "PropertyImages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "اسم المدينة المرتبطة بالصورة");

            // Create index for CityName lookups
            migrationBuilder.CreateIndex(
                name: "IX_PropertyImages_CityName",
                table: "PropertyImages",
                column: "CityName");

            // Add optional FK to Cities.Name (string PK)
            migrationBuilder.AddForeignKey(
                name: "FK_PropertyImages_Cities_CityName",
                table: "PropertyImages",
                column: "CityName",
                principalTable: "Cities",
                principalColumn: "Name",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyImages_Cities_CityName",
                table: "PropertyImages");

            migrationBuilder.DropIndex(
                name: "IX_PropertyImages_CityName",
                table: "PropertyImages");

            migrationBuilder.DropColumn(
                name: "CityName",
                table: "PropertyImages");
        }
    }
}

