using AspNetCoreDemo.Model.EFCore.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.EFCore
{
    public class BaseContext : DbContext, IBaseContext
    {
        public BaseContext(DbContextOptions options) : base(options)
        {

        }

        #region 基础表
        public DbSet<SysUser> SysUser { get; set; }
        public DbSet<OprLog> OprLog { get; set; }
        public DbSet<SMSInfo> SMSInfo { get; set; }
        public DbSet<Person> Person { get; set; }
        #endregion

        public DbSet<Student> Student { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<StudentCourse> StudentCourse { get; set; }
        public DbSet<StudentScore> StudentScore { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new EFLoggerProvider());
            optionsBuilder.UseLoggerFactory(loggerFactory);
#endif
            base.OnConfiguring(optionsBuilder);
        }
    }
}
