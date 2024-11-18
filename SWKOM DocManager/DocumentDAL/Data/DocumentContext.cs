using DocumentDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentDAL.Data
{
    public sealed class DocumentContext : DbContext
    {
        public DocumentContext(DbContextOptions<DocumentContext> options) : base(options)
        {
        }

        public DbSet<DocumentItem> DocumentItems { get; set; }
        public DbSet<DocumentContent> DocumentContents { get; set; }
        public DbSet<DocumentMetadata> DocumentMetadatas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the relationship between DocumentItem and DocumentContent
            modelBuilder.Entity<DocumentItem>()
                .HasOne(d => d.DocumentContent)
                .WithOne()
                .HasForeignKey<DocumentContent>(dc => dc.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between DocumentItem and DocumentMetadata
            modelBuilder.Entity<DocumentItem>()
                .HasOne(d => d.DocumentMetadata)
                .WithOne()
                .HasForeignKey<DocumentMetadata>(dm => dm.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DocumentMetadata>()
                .Property(d => d.UploadDate)
                .HasConversion(
                    v => v.ToUniversalTime(), // Convert to UTC when saving
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc).ToLocalTime() // Convert back to local time when reading
                );

            base.OnModelCreating(modelBuilder);
        }
        public void InitializeDatabase()
        {
            // This method can be called during application startup
            Database.Migrate();
        }
    }
}