using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemenBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixUnicodeForLogsAndReviewsSafe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasePricePerNight",
                table: "Properties");

            // Ensure review-related text columns are stored using Unicode to support Arabic content
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reviews') AND name = 'Comment')
ALTER TABLE dbo.Reviews ALTER COLUMN Comment NVARCHAR(MAX) NOT NULL;");

            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reviews') AND name = 'ResponseText')
ALTER TABLE dbo.Reviews ALTER COLUMN ResponseText NVARCHAR(MAX) NULL;");

            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ReviewResponses') AND name = 'Text')
ALTER TABLE dbo.ReviewResponses ALTER COLUMN Text NVARCHAR(MAX) NOT NULL;");

            // Audit log textual fields
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditLogs') AND name = 'Notes')
ALTER TABLE dbo.AuditLogs ALTER COLUMN Notes NVARCHAR(MAX) NULL;");

            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditLogs') AND name = 'OldValues')
ALTER TABLE dbo.AuditLogs ALTER COLUMN OldValues NVARCHAR(MAX) NULL;");

            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditLogs') AND name = 'NewValues')
ALTER TABLE dbo.AuditLogs ALTER COLUMN NewValues NVARCHAR(MAX) NULL;");

            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditLogs') AND name = 'Metadata')
ALTER TABLE dbo.AuditLogs ALTER COLUMN Metadata NVARCHAR(MAX) NULL;");

            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditLogs') AND name = 'Username')
ALTER TABLE dbo.AuditLogs ALTER COLUMN Username NVARCHAR(100) NULL;");

            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditLogs') AND name = 'EntityType')
ALTER TABLE dbo.AuditLogs ALTER COLUMN EntityType NVARCHAR(100) NOT NULL;");

            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditLogs') AND name = 'IpAddress')
ALTER TABLE dbo.AuditLogs ALTER COLUMN IpAddress NVARCHAR(50) NULL;");

            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditLogs') AND name = 'UserAgent')
ALTER TABLE dbo.AuditLogs ALTER COLUMN UserAgent NVARCHAR(255) NULL;");

            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditLogs') AND name = 'SessionId')
ALTER TABLE dbo.AuditLogs ALTER COLUMN SessionId NVARCHAR(100) NULL;");

            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditLogs') AND name = 'RequestId')
ALTER TABLE dbo.AuditLogs ALTER COLUMN RequestId NVARCHAR(100) NULL;");

            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditLogs') AND name = 'Source')
ALTER TABLE dbo.AuditLogs ALTER COLUMN Source NVARCHAR(100) NULL;");

            migrationBuilder.AlterColumn<string>(
                name: "ResponseText",
                table: "Reviews",
                type: "NVARCHAR(MAX)",
                nullable: true,
                comment: "نص رد التقييم",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ResponseText",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(MAX)",
                oldNullable: true,
                oldComment: "نص رد التقييم");

            migrationBuilder.AddColumn<decimal>(
                name: "BasePricePerNight",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

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
        }
    }
}
