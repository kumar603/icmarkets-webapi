/*
 * File: PaymentTransactionConfiguration.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Entity Framework Core configuration for PaymentTransaction entity mapping and indexing
 * 
 * Usage: Defines database Structure, relationships, and indexes for payment transactions.
 *        Called during DbContext model building to configure entity properties.
 *        Ensures efficient queries with composite indexes on status and date.
 *        Sets up required fields, constraints, and default values.
 * 
 * Dependencies: IEntityTypeConfiguration<PaymentTransaction>, PaymentTransaction
 */

using CIMarkets.Blockchain.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIMarkets.Blockchain.Infrastructure.Persistence
{
    /// <summary>
    /// Entity Framework Core configuration for PaymentTransaction entity.
    /// </summary>
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        /// <summary>
        /// Configures the PaymentTransaction entity mapping and indexes.
        /// </summary>
        /// <param name="builder">The entity type builder for PaymentTransaction.</param>
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            // Table configuration
            builder.ToTable("PaymentTransactions", "payment");

            // Primary key
            builder.HasKey(p => p.Id);

            // Properties configuration
            builder.Property(p => p.Id)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd();

            builder.Property(p => p.TransactionId)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("External payment gateway transaction ID");

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasPrecision(18, 8)
                .HasComment("Payment amount (supports 8 decimal places for crypto)");

            builder.Property(p => p.Currency)
                .IsRequired()
                .HasMaxLength(10)
                .HasComment("Currency code (USD, EUR, BTC, ETH, etc.)");

            builder.Property(p => p.Status)
                .IsRequired()
                .HasComment("Payment status (Pending=0, Processing=1, Completed=2, Failed=3, Cancelled=4, Refunded=5, Disputed=6)");

            builder.Property(p => p.PaymentMethod)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Payment method (card, wallet, crypto, bank_transfer)");

            builder.Property(p => p.BlockchainType)
                .IsRequired()
                .HasComment("Blockchain type for crypto payments (BTC=1, ETH=2, LTC=3, DASH=4)");

            builder.Property(p => p.CustomerId)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Customer identifier");

            builder.Property(p => p.GatewayResponse)
                .HasMaxLength(2000)
                .HasComment("Full response from payment gateway (JSON or XML)");

            builder.Property(p => p.ErrorMessage)
                .HasMaxLength(500)
                .HasComment("Error message if payment failed");

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Payment creation timestamp");

            builder.Property(p => p.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Last update timestamp");

            builder.Property(p => p.CompletedAt)
                .HasComment("Payment completion timestamp");

            builder.Property(p => p.Description)
                .HasMaxLength(500)
                .HasComment("Payment description for user reference");

            // Indexes for performance optimization
            builder.HasIndex(p => p.TransactionId)
                .IsUnique()
                .HasDatabaseName("IX_PaymentTransactions_TransactionId");

            builder.HasIndex(p => p.CustomerId)
                .HasDatabaseName("IX_PaymentTransactions_CustomerId");

            builder.HasIndex(p => new { p.Status, p.CreatedAt })
                .HasDatabaseName("IX_PaymentTransactions_Status_CreatedAt")
                .IsDescending(false, true);

            builder.HasIndex(p => p.CreatedAt)
                .HasDatabaseName("IX_PaymentTransactions_CreatedAt")
                .IsDescending();

            builder.HasIndex(p => new { p.CustomerId, p.Status })
                .HasDatabaseName("IX_PaymentTransactions_CustomerId_Status");
        }
    }
}
