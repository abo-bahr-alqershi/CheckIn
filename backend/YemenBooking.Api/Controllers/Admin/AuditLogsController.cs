using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.AuditLog;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace YemenBooking.Api.Controllers.Admin
{
    /// <summary>
    /// متحكم بسجلات نشاط وتدقيق النظام للمدراء
    /// Controller for admin activity and audit logs
    /// </summary>
    public class AuditLogsController : BaseAdminController
    {
        public AuditLogsController(IMediator mediator) : base(mediator) { }


        /// <summary>
        /// جلب سجلات التدقيق مع الفلاتر
        /// Get audit logs with optional filters (user, date range, search term, operation type)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAuditLogs([FromQuery] GetAuditLogsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// تشخيص بنية الأعمدة ونماذج بيانات نصية لضمان Unicode
        /// Diagnostics: check column types and sample text for Unicode
        /// </summary>
        [HttpGet("diagnostics/text-encoding")]
        public async Task<IActionResult> CheckTextEncoding([FromServices] IConfiguration configuration)
        {
            var connStr = configuration.GetConnectionString("DefaultConnection");
            using var sql = new SqlConnection(connStr);
            await sql.OpenAsync();

            var checkSql = @"
SELECT 'Reviews.Comment' AS ColumnName, t.name AS SqlType
FROM sys.columns c JOIN sys.types t ON c.user_type_id=t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Reviews') AND c.name='Comment'
UNION ALL
SELECT 'ReviewResponses.Text', t.name FROM sys.columns c JOIN sys.types t ON c.user_type_id=t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.ReviewResponses') AND c.name='Text'
UNION ALL
SELECT 'AuditLogs.Notes', t.name FROM sys.columns c JOIN sys.types t ON c.user_type_id=t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.AuditLogs') AND c.name='Notes'
UNION ALL
SELECT 'AuditLogs.OldValues', t.name FROM sys.columns c JOIN sys.types t ON c.user_type_id=t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.AuditLogs') AND c.name='OldValues'
UNION ALL
SELECT 'AuditLogs.NewValues', t.name FROM sys.columns c JOIN sys.types t ON c.user_type_id=t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.AuditLogs') AND c.name='NewValues';

SELECT TOP 3 Id, Comment FROM dbo.Reviews ORDER BY CreatedAt DESC;
SELECT TOP 3 Id, [Text] FROM dbo.ReviewResponses ORDER BY RespondedAt DESC;
SELECT TOP 3 Id, Notes, OldValues, NewValues FROM dbo.AuditLogs ORDER BY CreatedAt DESC;
";

            using var cmd = new SqlCommand(checkSql, sql);
            using var reader = await cmd.ExecuteReaderAsync();

            var payload = new System.Collections.Generic.Dictionary<string, object?>();

            // Result set 1: types
            var types = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object?>>();
            while (await reader.ReadAsync())
            {
                types.Add(new System.Collections.Generic.Dictionary<string, object?>
                {
                    ["column"] = reader.GetString(0),
                    ["type"] = reader.GetString(1)
                });
            }
            payload["columnTypes"] = types;

            // Result set 2: Reviews sample
            await reader.NextResultAsync();
            var reviews = new System.Collections.Generic.List<object?>();
            while (await reader.ReadAsync())
            {
                reviews.Add(new { Id = reader.GetGuid(0), Comment = reader.IsDBNull(1) ? null : reader.GetString(1) });
            }
            payload["reviewsSamples"] = reviews;

            // Result set 3: ReviewResponses sample
            await reader.NextResultAsync();
            var responses = new System.Collections.Generic.List<object?>();
            while (await reader.ReadAsync())
            {
                responses.Add(new { Id = reader.GetGuid(0), Text = reader.IsDBNull(1) ? null : reader.GetString(1) });
            }
            payload["reviewResponsesSamples"] = responses;

            // Result set 4: AuditLogs sample
            await reader.NextResultAsync();
            var audits = new System.Collections.Generic.List<object?>();
            while (await reader.ReadAsync())
            {
                audits.Add(new {
                    Id = reader.GetGuid(0),
                    Notes = reader.IsDBNull(1) ? null : reader.GetString(1),
                    OldValues = reader.IsDBNull(2) ? null : reader.GetString(2),
                    NewValues = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }
            payload["auditLogsSamples"] = audits;

            return Ok(payload);
        }

    }
} 