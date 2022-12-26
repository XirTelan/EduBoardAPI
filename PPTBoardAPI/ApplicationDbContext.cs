using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PPTBoardAPI.Authentication;
using PPTBoardAPI.Entities;
using System.Diagnostics.CodeAnalysis;

namespace PPTBoardAPI
{
    public class ApplicationDbContext : IdentityDbContext<Person>
    {
        public ApplicationDbContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<SpecialityDiscipline>().HasKey(x => new { x.SpecialityId, x.DisciplineId });
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<ControllRecord> ControllRecords { get; set; }
        public DbSet<ControllType> ControllTypes { get; set; }
    }
}
