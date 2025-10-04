using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemenBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Auto_20251004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatAttachments_ChatMessages_ChatMessageId",
                table: "ChatAttachments");

            migrationBuilder.RenameColumn(
                name: "ChatMessageId",
                table: "ChatAttachments",
                newName: "MessageId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatAttachments_ChatMessageId",
                table: "ChatAttachments",
                newName: "IX_ChatAttachments_MessageId");

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 626, DateTimeKind.Utc).AddTicks(5536), new DateTime(2025, 10, 4, 20, 54, 44, 626, DateTimeKind.Utc).AddTicks(5537) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 626, DateTimeKind.Utc).AddTicks(5551), new DateTime(2025, 10, 4, 20, 54, 44, 626, DateTimeKind.Utc).AddTicks(5551) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 626, DateTimeKind.Utc).AddTicks(5553), new DateTime(2025, 10, 4, 20, 54, 44, 626, DateTimeKind.Utc).AddTicks(5553) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 626, DateTimeKind.Utc).AddTicks(5555), new DateTime(2025, 10, 4, 20, 54, 44, 626, DateTimeKind.Utc).AddTicks(5555) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 626, DateTimeKind.Utc).AddTicks(5556), new DateTime(2025, 10, 4, 20, 54, 44, 626, DateTimeKind.Utc).AddTicks(5556) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 626, DateTimeKind.Utc).AddTicks(5558), new DateTime(2025, 10, 4, 20, 54, 44, 626, DateTimeKind.Utc).AddTicks(5559) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 626, DateTimeKind.Utc).AddTicks(5561), new DateTime(2025, 10, 4, 20, 54, 44, 626, DateTimeKind.Utc).AddTicks(5561) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 626, DateTimeKind.Utc).AddTicks(5562), new DateTime(2025, 10, 4, 20, 54, 44, 626, DateTimeKind.Utc).AddTicks(5563) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Code",
                keyValue: "USD",
                column: "LastUpdated",
                value: new DateTime(2025, 10, 4, 20, 54, 44, 675, DateTimeKind.Utc).AddTicks(4596));

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 546, DateTimeKind.Utc).AddTicks(8540), new DateTime(2025, 10, 4, 20, 54, 44, 546, DateTimeKind.Utc).AddTicks(8541) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 546, DateTimeKind.Utc).AddTicks(8576), new DateTime(2025, 10, 4, 20, 54, 44, 546, DateTimeKind.Utc).AddTicks(8577) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 546, DateTimeKind.Utc).AddTicks(8579), new DateTime(2025, 10, 4, 20, 54, 44, 546, DateTimeKind.Utc).AddTicks(8579) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 546, DateTimeKind.Utc).AddTicks(8581), new DateTime(2025, 10, 4, 20, 54, 44, 546, DateTimeKind.Utc).AddTicks(8581) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 546, DateTimeKind.Utc).AddTicks(8583), new DateTime(2025, 10, 4, 20, 54, 44, 546, DateTimeKind.Utc).AddTicks(8584) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 542, DateTimeKind.Utc).AddTicks(7075), new DateTime(2025, 10, 4, 20, 54, 44, 542, DateTimeKind.Utc).AddTicks(7076) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 542, DateTimeKind.Utc).AddTicks(7097), new DateTime(2025, 10, 4, 20, 54, 44, 542, DateTimeKind.Utc).AddTicks(7098) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 542, DateTimeKind.Utc).AddTicks(7101), new DateTime(2025, 10, 4, 20, 54, 44, 542, DateTimeKind.Utc).AddTicks(7102) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 542, DateTimeKind.Utc).AddTicks(7105), new DateTime(2025, 10, 4, 20, 54, 44, 542, DateTimeKind.Utc).AddTicks(7105) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 675, DateTimeKind.Utc).AddTicks(4688), new DateTime(2025, 10, 4, 20, 54, 44, 675, DateTimeKind.Utc).AddTicks(4688), new DateTime(2025, 10, 4, 20, 54, 44, 675, DateTimeKind.Utc).AddTicks(4688) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 675, DateTimeKind.Utc).AddTicks(4692), new DateTime(2025, 10, 4, 20, 54, 44, 675, DateTimeKind.Utc).AddTicks(4692), new DateTime(2025, 10, 4, 20, 54, 44, 675, DateTimeKind.Utc).AddTicks(4692) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 675, DateTimeKind.Utc).AddTicks(4448), new DateTime(2025, 10, 4, 20, 54, 44, 675, DateTimeKind.Utc).AddTicks(4449), new DateTime(2025, 10, 4, 20, 54, 44, 675, DateTimeKind.Utc).AddTicks(4420) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 20, 54, 44, 675, DateTimeKind.Utc).AddTicks(4474), new DateTime(2025, 10, 4, 20, 54, 44, 675, DateTimeKind.Utc).AddTicks(4475), new DateTime(2025, 10, 4, 20, 54, 44, 675, DateTimeKind.Utc).AddTicks(4472) });

            migrationBuilder.AddForeignKey(
                name: "FK_ChatAttachments_ChatMessages_MessageId",
                table: "ChatAttachments",
                column: "MessageId",
                principalTable: "ChatMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatAttachments_ChatMessages_MessageId",
                table: "ChatAttachments");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "ChatAttachments",
                newName: "ChatMessageId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatAttachments_MessageId",
                table: "ChatAttachments",
                newName: "IX_ChatAttachments_ChatMessageId");

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
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9211), new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9215), new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9201) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9236), new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9237), new DateTime(2025, 10, 4, 15, 1, 7, 59, DateTimeKind.Utc).AddTicks(9232) });

            migrationBuilder.AddForeignKey(
                name: "FK_ChatAttachments_ChatMessages_ChatMessageId",
                table: "ChatAttachments",
                column: "ChatMessageId",
                principalTable: "ChatMessages",
                principalColumn: "Id");
        }
    }
}
