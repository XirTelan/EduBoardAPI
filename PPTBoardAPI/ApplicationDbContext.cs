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

        public DbSet<Speciality> Specialities { get; set; }
    }
}
