using DocumentDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentDAL.Data
{
    public sealed class DocumentContext : DbContext
    {
        public DocumentContext(DbContextOptions<DocumentContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }

        public DbSet<DocumentItem> DocumentItems { get; set; }
        public DbSet<DocumentContent> DocumentContents { get; set; }
        public DbSet<DocumentMetadata> DocumentMetadatas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentItem>()
                .HasOne(d => d.DocumentContent)
                .WithOne(dc => dc.DocumentItem)
                .HasForeignKey<DocumentContent>(dc => dc.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DocumentItem>()
                .HasOne(d => d.DocumentMetadata)
                .WithOne(dc => dc.DocumentItem)
                .HasForeignKey<DocumentMetadata>(dc => dc.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
        public void InitializeDatabase()
        {
            // This method can be called during application startup
            Database.Migrate();
        }
    }
}