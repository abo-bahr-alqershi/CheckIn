using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemenBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Auto_20250923_061440 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CityName",
                table: "PropertyImages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "اسم المدينة المرتبطة بالصورة");

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

            migrationBuilder.CreateIndex(
                name: "IX_PropertyImages_CityName",
                table: "PropertyImages",
                column: "CityName");

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

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 81, DateTimeKind.Utc).AddTicks(5505), new DateTime(2025, 9, 3, 11, 59, 58, 81, DateTimeKind.Utc).AddTicks(5506) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 81, DateTimeKind.Utc).AddTicks(5518), new DateTime(2025, 9, 3, 11, 59, 58, 81, DateTimeKind.Utc).AddTicks(5518) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 81, DateTimeKind.Utc).AddTicks(5522), new DateTime(2025, 9, 3, 11, 59, 58, 81, DateTimeKind.Utc).AddTicks(5522) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 81, DateTimeKind.Utc).AddTicks(5525), new DateTime(2025, 9, 3, 11, 59, 58, 81, DateTimeKind.Utc).AddTicks(5526) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 81, DateTimeKind.Utc).AddTicks(5527), new DateTime(2025, 9, 3, 11, 59, 58, 81, DateTimeKind.Utc).AddTicks(5528) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 81, DateTimeKind.Utc).AddTicks(5530), new DateTime(2025, 9, 3, 11, 59, 58, 81, DateTimeKind.Utc).AddTicks(5542) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 81, DateTimeKind.Utc).AddTicks(5544), new DateTime(2025, 9, 3, 11, 59, 58, 81, DateTimeKind.Utc).AddTicks(5544) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 81, DateTimeKind.Utc).AddTicks(5547), new DateTime(2025, 9, 3, 11, 59, 58, 81, DateTimeKind.Utc).AddTicks(5547) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Code",
                keyValue: "USD",
                column: "LastUpdated",
                value: new DateTime(2025, 9, 3, 11, 59, 58, 127, DateTimeKind.Utc).AddTicks(6817));

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 3, DateTimeKind.Utc).AddTicks(4890), new DateTime(2025, 9, 3, 11, 59, 58, 3, DateTimeKind.Utc).AddTicks(4890) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 3, DateTimeKind.Utc).AddTicks(4903), new DateTime(2025, 9, 3, 11, 59, 58, 3, DateTimeKind.Utc).AddTicks(4903) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 3, DateTimeKind.Utc).AddTicks(4906), new DateTime(2025, 9, 3, 11, 59, 58, 3, DateTimeKind.Utc).AddTicks(4907) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 3, DateTimeKind.Utc).AddTicks(4912), new DateTime(2025, 9, 3, 11, 59, 58, 3, DateTimeKind.Utc).AddTicks(4912) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 3, DateTimeKind.Utc).AddTicks(4914), new DateTime(2025, 9, 3, 11, 59, 58, 3, DateTimeKind.Utc).AddTicks(4914) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 57, 999, DateTimeKind.Utc).AddTicks(9358), new DateTime(2025, 9, 3, 11, 59, 57, 999, DateTimeKind.Utc).AddTicks(9359) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 57, 999, DateTimeKind.Utc).AddTicks(9371), new DateTime(2025, 9, 3, 11, 59, 57, 999, DateTimeKind.Utc).AddTicks(9372) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 57, 999, DateTimeKind.Utc).AddTicks(9374), new DateTime(2025, 9, 3, 11, 59, 57, 999, DateTimeKind.Utc).AddTicks(9374) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 57, 999, DateTimeKind.Utc).AddTicks(9394), new DateTime(2025, 9, 3, 11, 59, 57, 999, DateTimeKind.Utc).AddTicks(9394) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 127, DateTimeKind.Utc).AddTicks(6904), new DateTime(2025, 9, 3, 11, 59, 58, 127, DateTimeKind.Utc).AddTicks(6904), new DateTime(2025, 9, 3, 11, 59, 58, 127, DateTimeKind.Utc).AddTicks(6905) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 127, DateTimeKind.Utc).AddTicks(6909), new DateTime(2025, 9, 3, 11, 59, 58, 127, DateTimeKind.Utc).AddTicks(6909), new DateTime(2025, 9, 3, 11, 59, 58, 127, DateTimeKind.Utc).AddTicks(6909) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 127, DateTimeKind.Utc).AddTicks(6723), new DateTime(2025, 9, 3, 11, 59, 58, 127, DateTimeKind.Utc).AddTicks(6725), new DateTime(2025, 9, 3, 11, 59, 58, 127, DateTimeKind.Utc).AddTicks(6712) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 11, 59, 58, 127, DateTimeKind.Utc).AddTicks(6738), new DateTime(2025, 9, 3, 11, 59, 58, 127, DateTimeKind.Utc).AddTicks(6739), new DateTime(2025, 9, 3, 11, 59, 58, 127, DateTimeKind.Utc).AddTicks(6735) });
        }
    }
}
