using Microsoft.EntityFrameworkCore;
using RuleEngine.Data.Entity;

namespace RuleEngine.Data
{
    public class RuleEngineDb : DbContext
    {
        public static string ConnectionString { get; set; }

        public DbSet<DRule> Rules { get; set; }
        public DbSet<DField> Fields { get; set; }
        public DbSet<DType> Types { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);

            base.OnConfiguring(optionsBuilder);
        }
    }
}
