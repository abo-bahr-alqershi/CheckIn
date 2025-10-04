using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.AspNetCore.Http;
using YemenBooking.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace YemenBooking.Infrastructure.Data.Context;

/// <summary>
/// Factory for design-time DbContext creation.
/// مولد سياق قاعدة البيانات في وقت التصميم
/// </summary>
public class YemenBookingDbContextFactory : IDesignTimeDbContextFactory<YemenBookingDbContext>
{
    public YemenBookingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<YemenBookingDbContext>();
        // // تهيئة الاتصال بقاعدة بيانات SQLite
        // optionsBuilder.UseSqlite("Data Source=YemenBooking.db");
        // تهيئة الاتصال بقاعدة بيانات SQL Server
        optionsBuilder.UseSqlServer("Data Source=SQL5107.site4now.net;Initial Catalog=db_abd8fd_bookn2;User Id=db_abd8fd_bookn2_admin;Password=Qaz123@Wsx123@");
        // For design-time, httpContextAccessor not used, passing new HttpContextAccessor instance
        return new YemenBookingDbContext(
            optionsBuilder.Options,
            new HttpContextAccessor());
    }
} 