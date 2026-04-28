using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SafeVault.Models;

namespace SafeVault.Data;

/// <summary>
/// Application database context that extends IdentityDbContext
/// to support ASP.NET Identity with custom ApplicationUser.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
                {
                    }

                        // Vault items stored securely
                            public DbSet<VaultItem> VaultItems { get; set; }

                                // Audit log for security events
                                    public DbSet<AuditLog> AuditLogs { get; set; }

                                        protected override void OnModelCreating(ModelBuilder builder)
                                            {
                                                    base.OnModelCreating(builder);

                                                            // Configure VaultItem
                                                                    builder.Entity<VaultItem>(entity =>
                                                                            {
                                                                                        entity.HasKey(e => e.Id);
                                                                                                    entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                                                                                                                entity.Property(e => e.EncryptedContent).IsRequired();
                                                                                                                            entity.Property(e => e.UserId).IsRequired();
                                                                                                                                        entity.HasIndex(e => e.UserId);
                                                                                                                                                });
                                                                                                                                                
                                                                                                                                                        // Configure AuditLog
                                                                                                                                                                builder.Entity<AuditLog>(entity =>
                                                                                                                                                                        {
                                                                                                                                                                                    entity.HasKey(e => e.Id);
                                                                                                                                                                                                entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
                                                                                                                                                                                                            entity.Property(e => e.UserId).HasMaxLength(450);
                                                                                                                                                                                                                        entity.HasIndex(e => e.Timestamp);
                                                                                                                                                                                                                                });
                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                    }
