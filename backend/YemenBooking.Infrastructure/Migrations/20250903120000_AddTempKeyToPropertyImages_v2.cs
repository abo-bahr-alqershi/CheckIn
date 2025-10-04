using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemenBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTempKeyToPropertyImages_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RespondedBy",
                table: "ReviewResponses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "المستخدم الذي قام بالرد");

            migrationBuilder.AddColumn<string>(
                name: "RespondedByName",
                table: "ReviewResponses",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                comment: "اسم المجيب (منسوخ)");

            migrationBuilder.AddColumn<string>(
                name: "TempKey",
                table: "PropertyImages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "مفتاح مؤقت لرفع الصور قبل الربط");

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

            migrationBuilder.CreateIndex(
                name: "IX_PropertyImages_TempKey",
                table: "PropertyImages",
                column: "TempKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PropertyImages_TempKey",
                table: "PropertyImages");

            migrationBuilder.DropColumn(
                name: "RespondedBy",
                table: "ReviewResponses");

            migrationBuilder.DropColumn(
                name: "RespondedByName",
                table: "ReviewResponses");

            migrationBuilder.DropColumn(
                name: "TempKey",
                table: "PropertyImages");

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 901, DateTimeKind.Utc).AddTicks(8590), new DateTime(2025, 8, 30, 3, 12, 23, 901, DateTimeKind.Utc).AddTicks(8590) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 901, DateTimeKind.Utc).AddTicks(8602), new DateTime(2025, 8, 30, 3, 12, 23, 901, DateTimeKind.Utc).AddTicks(8602) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 901, DateTimeKind.Utc).AddTicks(8604), new DateTime(2025, 8, 30, 3, 12, 23, 901, DateTimeKind.Utc).AddTicks(8605) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 901, DateTimeKind.Utc).AddTicks(8606), new DateTime(2025, 8, 30, 3, 12, 23, 901, DateTimeKind.Utc).AddTicks(8607) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 901, DateTimeKind.Utc).AddTicks(8609), new DateTime(2025, 8, 30, 3, 12, 23, 901, DateTimeKind.Utc).AddTicks(8609) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 901, DateTimeKind.Utc).AddTicks(8610), new DateTime(2025, 8, 30, 3, 12, 23, 901, DateTimeKind.Utc).AddTicks(8615) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 901, DateTimeKind.Utc).AddTicks(8618), new DateTime(2025, 8, 30, 3, 12, 23, 901, DateTimeKind.Utc).AddTicks(8618) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 901, DateTimeKind.Utc).AddTicks(8725), new DateTime(2025, 8, 30, 3, 12, 23, 901, DateTimeKind.Utc).AddTicks(8726) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Code",
                keyValue: "USD",
                column: "LastUpdated",
                value: new DateTime(2025, 8, 30, 3, 12, 23, 936, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 853, DateTimeKind.Utc).AddTicks(7894), new DateTime(2025, 8, 30, 3, 12, 23, 853, DateTimeKind.Utc).AddTicks(7895) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 853, DateTimeKind.Utc).AddTicks(7909), new DateTime(2025, 8, 30, 3, 12, 23, 853, DateTimeKind.Utc).AddTicks(7909) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 853, DateTimeKind.Utc).AddTicks(7911), new DateTime(2025, 8, 30, 3, 12, 23, 853, DateTimeKind.Utc).AddTicks(7912) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 853, DateTimeKind.Utc).AddTicks(7914), new DateTime(2025, 8, 30, 3, 12, 23, 853, DateTimeKind.Utc).AddTicks(7914) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 853, DateTimeKind.Utc).AddTicks(7916), new DateTime(2025, 8, 30, 3, 12, 23, 853, DateTimeKind.Utc).AddTicks(7916) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 851, DateTimeKind.Utc).AddTicks(9496), new DateTime(2025, 8, 30, 3, 12, 23, 851, DateTimeKind.Utc).AddTicks(9496) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 851, DateTimeKind.Utc).AddTicks(9509), new DateTime(2025, 8, 30, 3, 12, 23, 851, DateTimeKind.Utc).AddTicks(9509) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 851, DateTimeKind.Utc).AddTicks(9511), new DateTime(2025, 8, 30, 3, 12, 23, 851, DateTimeKind.Utc).AddTicks(9511) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 851, DateTimeKind.Utc).AddTicks(9513), new DateTime(2025, 8, 30, 3, 12, 23, 851, DateTimeKind.Utc).AddTicks(9513) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 936, DateTimeKind.Utc).AddTicks(5283), new DateTime(2025, 8, 30, 3, 12, 23, 936, DateTimeKind.Utc).AddTicks(5283), new DateTime(2025, 8, 30, 3, 12, 23, 936, DateTimeKind.Utc).AddTicks(5283) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 936, DateTimeKind.Utc).AddTicks(5286), new DateTime(2025, 8, 30, 3, 12, 23, 936, DateTimeKind.Utc).AddTicks(5287), new DateTime(2025, 8, 30, 3, 12, 23, 936, DateTimeKind.Utc).AddTicks(5287) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 936, DateTimeKind.Utc).AddTicks(5137), new DateTime(2025, 8, 30, 3, 12, 23, 936, DateTimeKind.Utc).AddTicks(5141), new DateTime(2025, 8, 30, 3, 12, 23, 936, DateTimeKind.Utc).AddTicks(5119) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 3, 12, 23, 936, DateTimeKind.Utc).AddTicks(5167), new DateTime(2025, 8, 30, 3, 12, 23, 936, DateTimeKind.Utc).AddTicks(5167), new DateTime(2025, 8, 30, 3, 12, 23, 936, DateTimeKind.Utc).AddTicks(5163) });
        }
    }
}
