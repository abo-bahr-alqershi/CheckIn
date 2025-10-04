using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemenBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Auto_20250925 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationSeconds",
                table: "PropertyImages",
                type: "int",
                nullable: true,
                comment: "مدة الفيديو بالثواني");

            migrationBuilder.AddColumn<string>(
                name: "MediaType",
                table: "PropertyImages",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "image",
                comment: "نوع الوسائط image/video");

            migrationBuilder.AddColumn<string>(
                name: "VideoThumbnailUrl",
                table: "PropertyImages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "رابط المصغرة للفيديو");

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 860, DateTimeKind.Utc).AddTicks(7006), new DateTime(2025, 9, 25, 1, 37, 38, 860, DateTimeKind.Utc).AddTicks(7007) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 860, DateTimeKind.Utc).AddTicks(7022), new DateTime(2025, 9, 25, 1, 37, 38, 860, DateTimeKind.Utc).AddTicks(7022) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 860, DateTimeKind.Utc).AddTicks(7024), new DateTime(2025, 9, 25, 1, 37, 38, 860, DateTimeKind.Utc).AddTicks(7024) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 860, DateTimeKind.Utc).AddTicks(7025), new DateTime(2025, 9, 25, 1, 37, 38, 860, DateTimeKind.Utc).AddTicks(7026) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 860, DateTimeKind.Utc).AddTicks(7027), new DateTime(2025, 9, 25, 1, 37, 38, 860, DateTimeKind.Utc).AddTicks(7027) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 860, DateTimeKind.Utc).AddTicks(7030), new DateTime(2025, 9, 25, 1, 37, 38, 860, DateTimeKind.Utc).AddTicks(7037) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 860, DateTimeKind.Utc).AddTicks(7039), new DateTime(2025, 9, 25, 1, 37, 38, 860, DateTimeKind.Utc).AddTicks(7039) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 860, DateTimeKind.Utc).AddTicks(7041), new DateTime(2025, 9, 25, 1, 37, 38, 860, DateTimeKind.Utc).AddTicks(7041) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Code",
                keyValue: "USD",
                column: "LastUpdated",
                value: new DateTime(2025, 9, 25, 1, 37, 38, 906, DateTimeKind.Utc).AddTicks(8710));

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 799, DateTimeKind.Utc).AddTicks(7792), new DateTime(2025, 9, 25, 1, 37, 38, 799, DateTimeKind.Utc).AddTicks(7792) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 799, DateTimeKind.Utc).AddTicks(7801), new DateTime(2025, 9, 25, 1, 37, 38, 799, DateTimeKind.Utc).AddTicks(7801) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 799, DateTimeKind.Utc).AddTicks(7803), new DateTime(2025, 9, 25, 1, 37, 38, 799, DateTimeKind.Utc).AddTicks(7803) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 799, DateTimeKind.Utc).AddTicks(7805), new DateTime(2025, 9, 25, 1, 37, 38, 799, DateTimeKind.Utc).AddTicks(7805) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 799, DateTimeKind.Utc).AddTicks(7807), new DateTime(2025, 9, 25, 1, 37, 38, 799, DateTimeKind.Utc).AddTicks(7808) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 797, DateTimeKind.Utc).AddTicks(4823), new DateTime(2025, 9, 25, 1, 37, 38, 797, DateTimeKind.Utc).AddTicks(4824) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 797, DateTimeKind.Utc).AddTicks(4836), new DateTime(2025, 9, 25, 1, 37, 38, 797, DateTimeKind.Utc).AddTicks(4836) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 797, DateTimeKind.Utc).AddTicks(4837), new DateTime(2025, 9, 25, 1, 37, 38, 797, DateTimeKind.Utc).AddTicks(4838) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 797, DateTimeKind.Utc).AddTicks(4839), new DateTime(2025, 9, 25, 1, 37, 38, 797, DateTimeKind.Utc).AddTicks(4839) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 906, DateTimeKind.Utc).AddTicks(8774), new DateTime(2025, 9, 25, 1, 37, 38, 906, DateTimeKind.Utc).AddTicks(8774), new DateTime(2025, 9, 25, 1, 37, 38, 906, DateTimeKind.Utc).AddTicks(8775) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 906, DateTimeKind.Utc).AddTicks(8778), new DateTime(2025, 9, 25, 1, 37, 38, 906, DateTimeKind.Utc).AddTicks(8778), new DateTime(2025, 9, 25, 1, 37, 38, 906, DateTimeKind.Utc).AddTicks(8779) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 906, DateTimeKind.Utc).AddTicks(8579), new DateTime(2025, 9, 25, 1, 37, 38, 906, DateTimeKind.Utc).AddTicks(8580), new DateTime(2025, 9, 25, 1, 37, 38, 906, DateTimeKind.Utc).AddTicks(8565) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 1, 37, 38, 906, DateTimeKind.Utc).AddTicks(8648), new DateTime(2025, 9, 25, 1, 37, 38, 906, DateTimeKind.Utc).AddTicks(8649), new DateTime(2025, 9, 25, 1, 37, 38, 906, DateTimeKind.Utc).AddTicks(8645) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "PropertyImages");

            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "PropertyImages");

            migrationBuilder.DropColumn(
                name: "VideoThumbnailUrl",
                table: "PropertyImages");

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 796, DateTimeKind.Utc).AddTicks(8365), new DateTime(2025, 9, 23, 3, 14, 53, 796, DateTimeKind.Utc).AddTicks(8366) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 796, DateTimeKind.Utc).AddTicks(8378), new DateTime(2025, 9, 23, 3, 14, 53, 796, DateTimeKind.Utc).AddTicks(8378) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 796, DateTimeKind.Utc).AddTicks(8380), new DateTime(2025, 9, 23, 3, 14, 53, 796, DateTimeKind.Utc).AddTicks(8381) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 796, DateTimeKind.Utc).AddTicks(8382), new DateTime(2025, 9, 23, 3, 14, 53, 796, DateTimeKind.Utc).AddTicks(8382) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 796, DateTimeKind.Utc).AddTicks(8384), new DateTime(2025, 9, 23, 3, 14, 53, 796, DateTimeKind.Utc).AddTicks(8384) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 796, DateTimeKind.Utc).AddTicks(8386), new DateTime(2025, 9, 23, 3, 14, 53, 796, DateTimeKind.Utc).AddTicks(8393) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 796, DateTimeKind.Utc).AddTicks(8395), new DateTime(2025, 9, 23, 3, 14, 53, 796, DateTimeKind.Utc).AddTicks(8395) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 796, DateTimeKind.Utc).AddTicks(8397), new DateTime(2025, 9, 23, 3, 14, 53, 796, DateTimeKind.Utc).AddTicks(8397) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Code",
                keyValue: "USD",
                column: "LastUpdated",
                value: new DateTime(2025, 9, 23, 3, 14, 53, 839, DateTimeKind.Utc).AddTicks(2818));

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 738, DateTimeKind.Utc).AddTicks(4316), new DateTime(2025, 9, 23, 3, 14, 53, 738, DateTimeKind.Utc).AddTicks(4317) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 738, DateTimeKind.Utc).AddTicks(4326), new DateTime(2025, 9, 23, 3, 14, 53, 738, DateTimeKind.Utc).AddTicks(4326) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 738, DateTimeKind.Utc).AddTicks(4328), new DateTime(2025, 9, 23, 3, 14, 53, 738, DateTimeKind.Utc).AddTicks(4328) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 738, DateTimeKind.Utc).AddTicks(4330), new DateTime(2025, 9, 23, 3, 14, 53, 738, DateTimeKind.Utc).AddTicks(4330) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 738, DateTimeKind.Utc).AddTicks(4332), new DateTime(2025, 9, 23, 3, 14, 53, 738, DateTimeKind.Utc).AddTicks(4332) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 736, DateTimeKind.Utc).AddTicks(2590), new DateTime(2025, 9, 23, 3, 14, 53, 736, DateTimeKind.Utc).AddTicks(2591) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 736, DateTimeKind.Utc).AddTicks(2603), new DateTime(2025, 9, 23, 3, 14, 53, 736, DateTimeKind.Utc).AddTicks(2603) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 736, DateTimeKind.Utc).AddTicks(2605), new DateTime(2025, 9, 23, 3, 14, 53, 736, DateTimeKind.Utc).AddTicks(2605) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 736, DateTimeKind.Utc).AddTicks(2606), new DateTime(2025, 9, 23, 3, 14, 53, 736, DateTimeKind.Utc).AddTicks(2607) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 839, DateTimeKind.Utc).AddTicks(2893), new DateTime(2025, 9, 23, 3, 14, 53, 839, DateTimeKind.Utc).AddTicks(2893), new DateTime(2025, 9, 23, 3, 14, 53, 839, DateTimeKind.Utc).AddTicks(2894) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 839, DateTimeKind.Utc).AddTicks(2897), new DateTime(2025, 9, 23, 3, 14, 53, 839, DateTimeKind.Utc).AddTicks(2897), new DateTime(2025, 9, 23, 3, 14, 53, 839, DateTimeKind.Utc).AddTicks(2897) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 839, DateTimeKind.Utc).AddTicks(2748), new DateTime(2025, 9, 23, 3, 14, 53, 839, DateTimeKind.Utc).AddTicks(2749), new DateTime(2025, 9, 23, 3, 14, 53, 839, DateTimeKind.Utc).AddTicks(2732) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 3, 14, 53, 839, DateTimeKind.Utc).AddTicks(2761), new DateTime(2025, 9, 23, 3, 14, 53, 839, DateTimeKind.Utc).AddTicks(2761), new DateTime(2025, 9, 23, 3, 14, 53, 839, DateTimeKind.Utc).AddTicks(2759) });
        }
    }
}
