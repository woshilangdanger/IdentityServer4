using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Map;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Entity;

namespace Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ClassModel> Classs { get; set; }
        public DbSet<CourseModel> Courses { get; set; }
        public DbSet<ExamInfoModel> ExamInfos { get; set; }
        public DbSet<RoomModel> Rooms { get; set; }
        public DbSet<TeacherModel> Teachers { get; set; }

        public DbSet<BoxModel> Boxs { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ClassMap());
            builder.ApplyConfiguration(new CourseMap());
            builder.ApplyConfiguration(new ExamInfoMap());
            builder.ApplyConfiguration(new RoomMap());
            builder.ApplyConfiguration(new TeacherMap());
            builder.ApplyConfiguration(new BoxMap());
            base.OnModelCreating(builder);
        }
    }
}
