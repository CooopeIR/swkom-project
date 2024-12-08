using DocumentDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentDAL.Data
{
    /// <summary>
    /// Document Context methods with relationships and database imnitialization
    /// </summary>
    public sealed class DocumentContext : DbContext
    {
        /// <summary>
        /// Constructor for DocumentContext Class
        /// </summary>
        /// <param name="options"></param>
        public DocumentContext(DbContextOptions<DocumentContext> options) : base(options)
        {
        }

        /// <summary>
        /// DocumentItem DBSet for queries against the database
        /// </summary>
        public DbSet<DocumentItem> DocumentItems { get; set; }

        /// <summary>
        /// DocumentContent DBSet for queries against the database
        /// </summary>
        public DbSet<DocumentContent> DocumentContents { get; set; }

        /// <summary>
        /// DocumentMetadata DBSet for queries against the database
        /// </summary>
        public DbSet<DocumentMetadata> DocumentMetadatas { get; set; }

        /// <summary>
        /// Specify the relationships between DocumentItem and DocumentContent, DocumentItem and DocumentMetadata
        /// </summary>
        /// <param name="modelBuilder"></param>
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

        /// <summary>
        /// Method for Database initialization
        /// </summary>
        public void InitializeDatabase()
        {
            // This method can be called during application startup
            Database.Migrate();
        }
    }
}