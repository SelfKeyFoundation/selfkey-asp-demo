using Microsoft.EntityFrameworkCore;
using SelfKey.Login.Data.Models;

namespace SelfKey.Login.Service.Controllers
{
    public class SelfKeyContext : DbContext
    {
        public SelfKeyContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Payload> Payloads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Payload>().Ignore(p => p.Type);
        }
    }
}
