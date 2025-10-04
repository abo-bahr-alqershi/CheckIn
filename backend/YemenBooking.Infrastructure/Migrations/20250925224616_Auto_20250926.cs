using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemenBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Auto_20250926 : Migration
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

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2390), new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2391) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2407), new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2408) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2411), new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2412) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2415), new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2416) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2419), new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2419) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2422), new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2423) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2427), new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2427) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2430), new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2431) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Code",
                keyValue: "USD",
                column: "LastUpdated",
                value: new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(3897));

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4561), new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4561) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4581), new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4582) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4584), new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4584) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4587), new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4587) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4589), new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4589) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 27, DateTimeKind.Utc).AddTicks(2702), new DateTime(2025, 9, 25, 22, 46, 15, 27, DateTimeKind.Utc).AddTicks(2703) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 27, DateTimeKind.Utc).AddTicks(2718), new DateTime(2025, 9, 25, 22, 46, 15, 27, DateTimeKind.Utc).AddTicks(2718) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 27, DateTimeKind.Utc).AddTicks(2719), new DateTime(2025, 9, 25, 22, 46, 15, 27, DateTimeKind.Utc).AddTicks(2719) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 27, DateTimeKind.Utc).AddTicks(2721), new DateTime(2025, 9, 25, 22, 46, 15, 27, DateTimeKind.Utc).AddTicks(2721) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(4161), new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(4161), new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(4161) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(4168), new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(4168), new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(4169) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(3782), new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(3783), new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(3762) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(3803), new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(3804), new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(3799) });

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
    }
}
