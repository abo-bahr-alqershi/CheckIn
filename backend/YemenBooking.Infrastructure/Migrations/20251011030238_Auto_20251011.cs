using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemenBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Auto_20251011 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PropertyServices",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "وصف الخدمة");

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 490, DateTimeKind.Utc).AddTicks(1008), new DateTime(2025, 10, 11, 3, 2, 36, 490, DateTimeKind.Utc).AddTicks(1009) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 490, DateTimeKind.Utc).AddTicks(1027), new DateTime(2025, 10, 11, 3, 2, 36, 490, DateTimeKind.Utc).AddTicks(1027) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 490, DateTimeKind.Utc).AddTicks(1030), new DateTime(2025, 10, 11, 3, 2, 36, 490, DateTimeKind.Utc).AddTicks(1030) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 490, DateTimeKind.Utc).AddTicks(1032), new DateTime(2025, 10, 11, 3, 2, 36, 490, DateTimeKind.Utc).AddTicks(1032) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 490, DateTimeKind.Utc).AddTicks(1034), new DateTime(2025, 10, 11, 3, 2, 36, 490, DateTimeKind.Utc).AddTicks(1034) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 490, DateTimeKind.Utc).AddTicks(1037), new DateTime(2025, 10, 11, 3, 2, 36, 490, DateTimeKind.Utc).AddTicks(1037) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 490, DateTimeKind.Utc).AddTicks(1040), new DateTime(2025, 10, 11, 3, 2, 36, 490, DateTimeKind.Utc).AddTicks(1040) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 490, DateTimeKind.Utc).AddTicks(1042), new DateTime(2025, 10, 11, 3, 2, 36, 490, DateTimeKind.Utc).AddTicks(1043) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Code",
                keyValue: "USD",
                column: "LastUpdated",
                value: new DateTime(2025, 10, 11, 3, 2, 36, 595, DateTimeKind.Utc).AddTicks(8915));

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 401, DateTimeKind.Utc).AddTicks(8773), new DateTime(2025, 10, 11, 3, 2, 36, 401, DateTimeKind.Utc).AddTicks(8774) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 401, DateTimeKind.Utc).AddTicks(8831), new DateTime(2025, 10, 11, 3, 2, 36, 401, DateTimeKind.Utc).AddTicks(8831) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 401, DateTimeKind.Utc).AddTicks(8835), new DateTime(2025, 10, 11, 3, 2, 36, 401, DateTimeKind.Utc).AddTicks(8836) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 401, DateTimeKind.Utc).AddTicks(8839), new DateTime(2025, 10, 11, 3, 2, 36, 401, DateTimeKind.Utc).AddTicks(8839) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 401, DateTimeKind.Utc).AddTicks(8842), new DateTime(2025, 10, 11, 3, 2, 36, 401, DateTimeKind.Utc).AddTicks(8842) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 396, DateTimeKind.Utc).AddTicks(8743), new DateTime(2025, 10, 11, 3, 2, 36, 396, DateTimeKind.Utc).AddTicks(8744) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 396, DateTimeKind.Utc).AddTicks(8763), new DateTime(2025, 10, 11, 3, 2, 36, 396, DateTimeKind.Utc).AddTicks(8763) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 396, DateTimeKind.Utc).AddTicks(8765), new DateTime(2025, 10, 11, 3, 2, 36, 396, DateTimeKind.Utc).AddTicks(8765) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 396, DateTimeKind.Utc).AddTicks(8767), new DateTime(2025, 10, 11, 3, 2, 36, 396, DateTimeKind.Utc).AddTicks(8767) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 595, DateTimeKind.Utc).AddTicks(9062), new DateTime(2025, 10, 11, 3, 2, 36, 595, DateTimeKind.Utc).AddTicks(9062), new DateTime(2025, 10, 11, 3, 2, 36, 595, DateTimeKind.Utc).AddTicks(9063) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 595, DateTimeKind.Utc).AddTicks(9072), new DateTime(2025, 10, 11, 3, 2, 36, 595, DateTimeKind.Utc).AddTicks(9072), new DateTime(2025, 10, 11, 3, 2, 36, 595, DateTimeKind.Utc).AddTicks(9073) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 595, DateTimeKind.Utc).AddTicks(8725), new DateTime(2025, 10, 11, 3, 2, 36, 595, DateTimeKind.Utc).AddTicks(8726), new DateTime(2025, 10, 11, 3, 2, 36, 595, DateTimeKind.Utc).AddTicks(8705) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 3, 2, 36, 595, DateTimeKind.Utc).AddTicks(8763), new DateTime(2025, 10, 11, 3, 2, 36, 595, DateTimeKind.Utc).AddTicks(8764), new DateTime(2025, 10, 11, 3, 2, 36, 595, DateTimeKind.Utc).AddTicks(8759) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "PropertyServices");

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 392, DateTimeKind.Utc).AddTicks(7700), new DateTime(2025, 10, 10, 22, 43, 30, 392, DateTimeKind.Utc).AddTicks(7700) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 392, DateTimeKind.Utc).AddTicks(7719), new DateTime(2025, 10, 10, 22, 43, 30, 392, DateTimeKind.Utc).AddTicks(7719) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 392, DateTimeKind.Utc).AddTicks(7721), new DateTime(2025, 10, 10, 22, 43, 30, 392, DateTimeKind.Utc).AddTicks(7721) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 392, DateTimeKind.Utc).AddTicks(7723), new DateTime(2025, 10, 10, 22, 43, 30, 392, DateTimeKind.Utc).AddTicks(7723) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 392, DateTimeKind.Utc).AddTicks(7725), new DateTime(2025, 10, 10, 22, 43, 30, 392, DateTimeKind.Utc).AddTicks(7725) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 392, DateTimeKind.Utc).AddTicks(7727), new DateTime(2025, 10, 10, 22, 43, 30, 392, DateTimeKind.Utc).AddTicks(7727) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 392, DateTimeKind.Utc).AddTicks(7729), new DateTime(2025, 10, 10, 22, 43, 30, 392, DateTimeKind.Utc).AddTicks(7729) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 392, DateTimeKind.Utc).AddTicks(7731), new DateTime(2025, 10, 10, 22, 43, 30, 392, DateTimeKind.Utc).AddTicks(7731) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Code",
                keyValue: "USD",
                column: "LastUpdated",
                value: new DateTime(2025, 10, 10, 22, 43, 30, 444, DateTimeKind.Utc).AddTicks(1996));

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 307, DateTimeKind.Utc).AddTicks(2950), new DateTime(2025, 10, 10, 22, 43, 30, 307, DateTimeKind.Utc).AddTicks(2951) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 307, DateTimeKind.Utc).AddTicks(2983), new DateTime(2025, 10, 10, 22, 43, 30, 307, DateTimeKind.Utc).AddTicks(2983) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 307, DateTimeKind.Utc).AddTicks(2986), new DateTime(2025, 10, 10, 22, 43, 30, 307, DateTimeKind.Utc).AddTicks(2986) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 307, DateTimeKind.Utc).AddTicks(2988), new DateTime(2025, 10, 10, 22, 43, 30, 307, DateTimeKind.Utc).AddTicks(2989) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 307, DateTimeKind.Utc).AddTicks(2991), new DateTime(2025, 10, 10, 22, 43, 30, 307, DateTimeKind.Utc).AddTicks(2991) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 303, DateTimeKind.Utc).AddTicks(6375), new DateTime(2025, 10, 10, 22, 43, 30, 303, DateTimeKind.Utc).AddTicks(6376) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 303, DateTimeKind.Utc).AddTicks(6443), new DateTime(2025, 10, 10, 22, 43, 30, 303, DateTimeKind.Utc).AddTicks(6443) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 303, DateTimeKind.Utc).AddTicks(6447), new DateTime(2025, 10, 10, 22, 43, 30, 303, DateTimeKind.Utc).AddTicks(6448) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 303, DateTimeKind.Utc).AddTicks(6451), new DateTime(2025, 10, 10, 22, 43, 30, 303, DateTimeKind.Utc).AddTicks(6451) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 444, DateTimeKind.Utc).AddTicks(2075), new DateTime(2025, 10, 10, 22, 43, 30, 444, DateTimeKind.Utc).AddTicks(2076), new DateTime(2025, 10, 10, 22, 43, 30, 444, DateTimeKind.Utc).AddTicks(2076) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 444, DateTimeKind.Utc).AddTicks(2079), new DateTime(2025, 10, 10, 22, 43, 30, 444, DateTimeKind.Utc).AddTicks(2080), new DateTime(2025, 10, 10, 22, 43, 30, 444, DateTimeKind.Utc).AddTicks(2080) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 444, DateTimeKind.Utc).AddTicks(1908), new DateTime(2025, 10, 10, 22, 43, 30, 444, DateTimeKind.Utc).AddTicks(1909), new DateTime(2025, 10, 10, 22, 43, 30, 444, DateTimeKind.Utc).AddTicks(1890) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 22, 43, 30, 444, DateTimeKind.Utc).AddTicks(1922), new DateTime(2025, 10, 10, 22, 43, 30, 444, DateTimeKind.Utc).AddTicks(1923), new DateTime(2025, 10, 10, 22, 43, 30, 444, DateTimeKind.Utc).AddTicks(1920) });
        }
    }
}
