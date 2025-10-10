using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using YemenBooking.Infrastructure.Data.Context;

namespace YemenBooking.Infrastructure.Migrations;

// Enforce NVARCHAR on text columns that may contain Arabic characters
[DbContext(typeof(YemenBookingDbContext))]
[Migration("20251010201500_FixUnicodeForLogsAndReviews")]
public partial class FixUnicodeForLogsAndReviews : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Reviews
        migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reviews') AND name = 'Comment')
ALTER TABLE dbo.Reviews ALTER COLUMN Comment NVARCHAR(MAX) NOT NULL;");

        migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reviews') AND name = 'ResponseText')
ALTER TABLE dbo.Reviews ALTER COLUMN ResponseText NVARCHAR(MAX) NULL;");

        // ReviewResponses
        migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ReviewResponses') AND name = 'Text')
ALTER TABLE dbo.ReviewResponses ALTER COLUMN Text NVARCHAR(MAX) NOT NULL;");

        // AuditLogs - textual fields
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
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // No-op: keep Unicode types to avoid reintroducing data loss.
    }
}
