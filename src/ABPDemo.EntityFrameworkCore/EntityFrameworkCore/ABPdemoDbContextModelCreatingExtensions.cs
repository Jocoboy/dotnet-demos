using ABPDemo.StudentManagement;
using ABPDemo.SystemManagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace ABPDemo.EntityFrameworkCore
{
    public static class ABPdemoDbContextModelCreatingExtensions
    {
        public static void ConfigureSystemManagement(this ModelBuilder builder)
        {
            /* Configure your own tables/entities inside here */

            /*
             * builder.Entity<EntityA>(p =>
             * {
             *      p.HasOne(a => a.EntityB_Property).WithOne().HasForeignKey<EntityA>(a => a.EntityB_Id); // 一对一
             *      p.HasOne(a => a.EntityB_Property).WithMany(b => b.EntityA_Properties).HasForeignKey(a => a.EntityB_Id); // 一对多
             *      p.HasMany(a => a.EntityB_Properties).WithOne(b => b.EntityA_Property).HasForeignKey(b => b.EntityA_Id); // 多对一
             * }
             */

            builder.Entity<OperationLog>(b =>
            {
                b.ToTable(ABPDemoConsts.DbTablePrefix + "OperationLogs", ABPDemoConsts.DbSchema);
                b.Property(x => x.Data).HasColumnType("jsonb");
                b.ConfigureByConvention();
            });
        }

        public static void ConfigureStudentManagement(this ModelBuilder builder)
        {
            builder.Entity<Course>(b =>
            {
                b.ToTable(ABPDemoConsts.DbTablePrefix + "Courses", ABPDemoConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).IsRequired().HasMaxLength(100);
            });

            builder.Entity<Student>(b =>
            {
                b.ToTable(ABPDemoConsts.DbTablePrefix + "Students", ABPDemoConsts.DbSchema);
                b.HasMany(p => p.StudentScores).WithOne().HasForeignKey(p => p.StudentId);
                b.HasMany(p => p.StudentCourses).WithOne(p => p.Student).HasForeignKey(p => p.StudentId);
                b.ConfigureByConvention();
                b.Property(x => x.Name).IsRequired().HasMaxLength(20);
            });

            builder.Entity<StudentCourse>(b =>
            {
                b.ToTable(ABPDemoConsts.DbTablePrefix + "StudentCourses", ABPDemoConsts.DbSchema);
                b.HasKey(p => new { p.StudentId, p.CourseId });
                b.HasOne(p => p.Course).WithOne().HasForeignKey<StudentCourse>(p => p.CourseId);
                b.ConfigureByConvention();
            });

            builder.Entity<StudentScore>(b =>
            {
                b.ToTable(ABPDemoConsts.DbTablePrefix + "StudentScores", ABPDemoConsts.DbSchema);
                b.HasKey(p => new { p.StudentId, p.Year });
                b.ConfigureByConvention();
            });
        }
    }
}
