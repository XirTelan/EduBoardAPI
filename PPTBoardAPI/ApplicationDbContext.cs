using Microsoft.EntityFrameworkCore;
using PPTBoardAPI.Entities;
using System.Diagnostics.CodeAnalysis;

namespace PPTBoardAPI
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext([NotNullAttribute]DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<SpecialityDiscipline>().HasKey(x => new { x.SpecialityId, x.DisciplineId });
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<Student> Students { get; set; }
    }
}
