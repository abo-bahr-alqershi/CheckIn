using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemenBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreferredLanguage = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PreferredCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DarkMode = table.Column<bool>(type: "bit", nullable: false),
                    BookingNotifications = table.Column<bool>(type: "bit", nullable: false),
                    PromotionalNotifications = table.Column<bool>(type: "bit", nullable: false),
                    EmailNotifications = table.Column<bool>(type: "bit", nullable: false),
                    SmsNotifications = table.Column<bool>(type: "bit", nullable: false),
                    PushNotifications = table.Column<bool>(type: "bit", nullable: false),
                    AdditionalSettings = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 414, DateTimeKind.Utc).AddTicks(7851), new DateTime(2025, 9, 30, 7, 27, 40, 414, DateTimeKind.Utc).AddTicks(7851) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 414, DateTimeKind.Utc).AddTicks(7865), new DateTime(2025, 9, 30, 7, 27, 40, 414, DateTimeKind.Utc).AddTicks(7865) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 414, DateTimeKind.Utc).AddTicks(7866), new DateTime(2025, 9, 30, 7, 27, 40, 414, DateTimeKind.Utc).AddTicks(7867) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 414, DateTimeKind.Utc).AddTicks(7868), new DateTime(2025, 9, 30, 7, 27, 40, 414, DateTimeKind.Utc).AddTicks(7868) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 414, DateTimeKind.Utc).AddTicks(7870), new DateTime(2025, 9, 30, 7, 27, 40, 414, DateTimeKind.Utc).AddTicks(7870) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 414, DateTimeKind.Utc).AddTicks(7872), new DateTime(2025, 9, 30, 7, 27, 40, 414, DateTimeKind.Utc).AddTicks(7882) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 414, DateTimeKind.Utc).AddTicks(7884), new DateTime(2025, 9, 30, 7, 27, 40, 414, DateTimeKind.Utc).AddTicks(7884) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 414, DateTimeKind.Utc).AddTicks(7885), new DateTime(2025, 9, 30, 7, 27, 40, 414, DateTimeKind.Utc).AddTicks(7886) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Code",
                keyValue: "USD",
                column: "LastUpdated",
                value: new DateTime(2025, 9, 30, 7, 27, 40, 462, DateTimeKind.Utc).AddTicks(5473));

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 345, DateTimeKind.Utc).AddTicks(859), new DateTime(2025, 9, 30, 7, 27, 40, 345, DateTimeKind.Utc).AddTicks(859) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 345, DateTimeKind.Utc).AddTicks(877), new DateTime(2025, 9, 30, 7, 27, 40, 345, DateTimeKind.Utc).AddTicks(877) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 345, DateTimeKind.Utc).AddTicks(880), new DateTime(2025, 9, 30, 7, 27, 40, 345, DateTimeKind.Utc).AddTicks(880) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 345, DateTimeKind.Utc).AddTicks(882), new DateTime(2025, 9, 30, 7, 27, 40, 345, DateTimeKind.Utc).AddTicks(882) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 345, DateTimeKind.Utc).AddTicks(884), new DateTime(2025, 9, 30, 7, 27, 40, 345, DateTimeKind.Utc).AddTicks(884) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 342, DateTimeKind.Utc).AddTicks(3344), new DateTime(2025, 9, 30, 7, 27, 40, 342, DateTimeKind.Utc).AddTicks(3345) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 342, DateTimeKind.Utc).AddTicks(3364), new DateTime(2025, 9, 30, 7, 27, 40, 342, DateTimeKind.Utc).AddTicks(3364) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 342, DateTimeKind.Utc).AddTicks(3366), new DateTime(2025, 9, 30, 7, 27, 40, 342, DateTimeKind.Utc).AddTicks(3366) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 342, DateTimeKind.Utc).AddTicks(3368), new DateTime(2025, 9, 30, 7, 27, 40, 342, DateTimeKind.Utc).AddTicks(3368) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 462, DateTimeKind.Utc).AddTicks(5627), new DateTime(2025, 9, 30, 7, 27, 40, 462, DateTimeKind.Utc).AddTicks(5627), new DateTime(2025, 9, 30, 7, 27, 40, 462, DateTimeKind.Utc).AddTicks(5627) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 462, DateTimeKind.Utc).AddTicks(5630), new DateTime(2025, 9, 30, 7, 27, 40, 462, DateTimeKind.Utc).AddTicks(5630), new DateTime(2025, 9, 30, 7, 27, 40, 462, DateTimeKind.Utc).AddTicks(5631) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 462, DateTimeKind.Utc).AddTicks(5405), new DateTime(2025, 9, 30, 7, 27, 40, 462, DateTimeKind.Utc).AddTicks(5405), new DateTime(2025, 9, 30, 7, 27, 40, 462, DateTimeKind.Utc).AddTicks(5391) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 27, 40, 462, DateTimeKind.Utc).AddTicks(5417), new DateTime(2025, 9, 30, 7, 27, 40, 462, DateTimeKind.Utc).AddTicks(5417), new DateTime(2025, 9, 30, 7, 27, 40, 462, DateTimeKind.Utc).AddTicks(5415) });

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserId",
                table: "UserSettings",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5940), new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5941) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5953), new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5953) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5955), new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5955) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5957), new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5957) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5958), new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5959) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5961), new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5969) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(6026), new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(6026) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(6028), new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(6028) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Code",
                keyValue: "USD",
                column: "LastUpdated",
                value: new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3855));

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5372), new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5373) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5381), new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5381) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5383), new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5383) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5385), new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5385) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5387), new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5387) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 310, DateTimeKind.Utc).AddTicks(2065), new DateTime(2025, 9, 26, 11, 10, 54, 310, DateTimeKind.Utc).AddTicks(2065) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 310, DateTimeKind.Utc).AddTicks(2076), new DateTime(2025, 9, 26, 11, 10, 54, 310, DateTimeKind.Utc).AddTicks(2077) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 310, DateTimeKind.Utc).AddTicks(2078), new DateTime(2025, 9, 26, 11, 10, 54, 310, DateTimeKind.Utc).AddTicks(2078) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 310, DateTimeKind.Utc).AddTicks(2080), new DateTime(2025, 9, 26, 11, 10, 54, 310, DateTimeKind.Utc).AddTicks(2080) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3926), new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3926), new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3926) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3929), new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3930), new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3930) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3780), new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3781), new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3768) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3792), new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3793), new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3790) });
        }
    }
}
