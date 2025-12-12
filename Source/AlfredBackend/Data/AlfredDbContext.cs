using Microsoft.EntityFrameworkCore;
using AlfredBackend.Models;

namespace AlfredBackend.Data
{
    public class AlfredDbContext : DbContext
    {
        public AlfredDbContext(DbContextOptions<AlfredDbContext> options) : base(options)
        {
        }
        
        public DbSet<TwitchUser> TwitchUsers { get; set; }
        public DbSet<BotAccount> BotAccounts { get; set; }
        public DbSet<BotSettings> BotSettings { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // TwitchUser configuration
            modelBuilder.Entity<TwitchUser>(entity =>
            {
                entity.HasKey(e => e.TwitchId);
                entity.HasIndex(e => e.Username);
                entity.HasIndex(e => e.Email);
                
                // One-to-one relationship with BotSettings
                entity.HasOne(e => e.BotSettings)
                    .WithOne(s => s.User)
                    .HasForeignKey<BotSettings>(s => s.TwitchUserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // One-to-many relationship with BotAccounts
                entity.HasMany(e => e.BotAccounts)
                    .WithOne(b => b.Owner)
                    .HasForeignKey(b => b.OwnerTwitchUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // BotAccount configuration
            modelBuilder.Entity<BotAccount>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.BotTwitchId);
                entity.HasIndex(e => e.OwnerTwitchUserId);
            });
            
            // BotSettings configuration with owned entities
            modelBuilder.Entity<BotSettings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.TwitchUserId).IsUnique();
                
                // Configure Components as owned collection
                entity.OwnsMany(e => e.Components, component =>
                {
                    component.Property(c => c.Name).IsRequired().HasMaxLength(100);
                    component.Property(c => c.Description).HasMaxLength(500);
                });
                
                // Configure Timeouts as owned collection
                entity.OwnsMany(e => e.Timeouts, timeout =>
                {
                    timeout.Property(t => t.Name).IsRequired().HasMaxLength(100);
                    timeout.Property(t => t.Description).HasMaxLength(500);
                    timeout.Property(t => t.ValueSeconds).IsRequired();
                });
            });
        }
    }
}
