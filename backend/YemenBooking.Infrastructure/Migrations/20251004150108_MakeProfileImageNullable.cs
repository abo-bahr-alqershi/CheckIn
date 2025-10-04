using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemenBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeProfileImageNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProfileImageUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImage",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "رابط صورة الملف الشخصي (اختياري)",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 7, 3, DateTimeKind.Utc).AddTicks(2137), new DateTime(2025, 10, 4, 15, 1, 7, 3, DateTimeKind.Utc).AddTicks(2138) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 7, 3, DateTimeKind.Utc).AddTicks(2154), new DateTime(2025, 10, 4, 15, 1, 7, 3, DateTimeKind.Utc).AddTicks(2155) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 7, 3, DateTimeKind.Utc).AddTicks(2158), new DateTime(2025, 10, 4, 15, 1, 7, 3, DateTimeKind.Utc).AddTicks(2159) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 7, 3, DateTimeKind.Utc).AddTicks(2162), new DateTime(2025, 10, 4, 15, 1, 7, 3, DateTimeKind.Utc).AddTicks(2163) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 7, 3, DateTimeKind.Utc).AddTicks(2167), new DateTime(2025, 10, 4, 15, 1, 7, 3, DateTimeKind.Utc).AddTicks(2167) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 7, 3, DateTimeKind.Utc).AddTicks(2171), new DateTime(2025, 10, 4, 15, 1, 7, 3, DateTimeKind.Utc).AddTicks(2171) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 7, 3, DateTimeKind.Utc).AddTicks(2175), new DateTime(2025, 10, 4, 15, 1, 7, 3, DateTimeKind.Utc).AddTicks(2175) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 7, 3, DateTimeKind.Utc).AddTicks(2179), new DateTime(2025, 10, 4, 15, 1, 7, 3, DateTimeKind.Utc).AddTicks(2179) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Code",
                keyValue: "USD",
                column: "LastUpdated",
                value: new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9361));

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 6, 931, DateTimeKind.Utc).AddTicks(116), new DateTime(2025, 10, 4, 15, 1, 6, 931, DateTimeKind.Utc).AddTicks(116) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 6, 931, DateTimeKind.Utc).AddTicks(139), new DateTime(2025, 10, 4, 15, 1, 6, 931, DateTimeKind.Utc).AddTicks(139) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 6, 931, DateTimeKind.Utc).AddTicks(141), new DateTime(2025, 10, 4, 15, 1, 6, 931, DateTimeKind.Utc).AddTicks(142) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 6, 931, DateTimeKind.Utc).AddTicks(144), new DateTime(2025, 10, 4, 15, 1, 6, 931, DateTimeKind.Utc).AddTicks(145) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 6, 931, DateTimeKind.Utc).AddTicks(146), new DateTime(2025, 10, 4, 15, 1, 6, 931, DateTimeKind.Utc).AddTicks(147) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 6, 928, DateTimeKind.Utc).AddTicks(7569), new DateTime(2025, 10, 4, 15, 1, 6, 928, DateTimeKind.Utc).AddTicks(7570) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 6, 928, DateTimeKind.Utc).AddTicks(7584), new DateTime(2025, 10, 4, 15, 1, 6, 928, DateTimeKind.Utc).AddTicks(7584) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 6, 928, DateTimeKind.Utc).AddTicks(7586), new DateTime(2025, 10, 4, 15, 1, 6, 928, DateTimeKind.Utc).AddTicks(7586) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 6, 928, DateTimeKind.Utc).AddTicks(7588), new DateTime(2025, 10, 4, 15, 1, 6, 928, DateTimeKind.Utc).AddTicks(7588) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9551), new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9551), new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9552) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9558), new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9558), new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9559) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "City", "Country", "CreatedAt", "LastLoginDate", "TimeZoneId", "UpdatedAt" },
                values: new object[] { "Sana'a", "Yemen", new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9211), new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9215), "Asia/Aden", new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9201) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "City", "Country", "CreatedAt", "LastLoginDate", "TimeZoneId", "UpdatedAt" },
                values: new object[] { "Aden", "Yemen", new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9236), new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9237), "Asia/Aden", new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9232) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImageUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImage",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "رابط صورة الملف الشخصي (اختياري)");

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
        }
    }
}
