/*
 * File: BlockchainRecordConfiguration.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Entity Framework Core fluent configuration for BlockchainRecord
 * 
 * Usage: Configures the BlockchainRecord entity mapping for database.
 *        Defines table name, column mappings, composite index on (BlockchainType, CreatedAt).
 *        Applied by BlockchainDbContext via modelBuilder.ApplyConfiguration().
 * 
 * Dependencies: BlockchainRecord entity, EntityTypeBuilder, Microsoft.EntityFrameworkCore
 */

using CIMarkets.Blockchain.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIMarkets.Blockchain.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for BlockchainRecord.
    /// </summary>
    public class BlockchainRecordConfiguration : IEntityTypeConfiguration<BlockchainRecord>
    {
        public void Configure(EntityTypeBuilder<BlockchainRecord> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .HasColumnName("Id")
                .IsRequired();

            builder.Property(r => r.BlockchainType)
                .HasColumnName("BlockchainType")
                .IsRequired();

            builder.Property(r => r.RawJson)
                .HasColumnName("RawJson")
                .IsRequired()
                .HasColumnType("TEXT");

            builder.Property(r => r.CreatedAt)
                .HasColumnName("CreatedAt")
                .IsRequired()
                .HasDefaultValueSql("DATETIME('now', 'utc')");

            // Create index for efficient history queries (sorted by CreatedAt descending)
            builder.HasIndex(r => new { r.BlockchainType, r.CreatedAt })
                .HasDatabaseName("IX_BlockchainRecords_Type_CreatedAt");

            builder.ToTable("BlockchainRecords");
        }
    }
}
