using Microsoft.EntityFrameworkCore;
using DocumentDAL.Entities;

namespace DocumentDAL.Data
{
    public sealed class DocumentContext(DbContextOptions<DocumentContext> options) : DbContext(options)
    {
        public DbSet<DocumentItem>? DocumentItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Manuelle Konfiguration der Tabelle
            modelBuilder.Entity<DocumentItem>(entity =>
            {
                entity.ToTable("DocumentItems");  // Setzt den Tabellennamen

                entity.HasKey(e => e.Id);  // Setzt den Primärschlüssel


                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);  // Konfiguriert den "Name"-Spalten

                entity.Property(e => e.Author);  // Konfiguriert die "IsComplete"-Spalte
                entity.Property(e => e.Date);  // Konfiguriert die "IsComplete"-Spalte
                entity.Property(e => e.Content);  // Konfiguriert die "IsComplete"-Spalte
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}