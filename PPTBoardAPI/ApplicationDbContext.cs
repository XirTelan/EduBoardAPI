using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PPTBoardAPI.DTOs;
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
            modelBuilder.Entity<Group>().HasOne(g => g.Speciality).WithMany(s => s.Groups).HasForeignKey(g => g.SpecialityId);
            modelBuilder.Entity<Group>().HasMany(g => g.Students).WithOne(s => s.Group).HasForeignKey(s => s.GroupId).OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<Student> Students { get; set; }
    }
}
