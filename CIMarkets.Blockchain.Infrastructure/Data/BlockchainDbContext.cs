using CIMarkets.Blockchain.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CIMarkets.Blockchain.Infrastructure.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for Blockchain data.
    /// </summary>
    public class BlockchainDbContext : DbContext
    {
        public BlockchainDbContext(DbContextOptions<BlockchainDbContext> options)
            : base(options)
        {
        }

        public DbSet<BlockchainRecord> BlockchainRecords { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations
            modelBuilder.ApplyConfiguration(new Configurations.BlockchainRecordConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PaymentTransactionConfiguration());
        }
    }
}
