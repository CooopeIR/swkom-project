using DocumentDAL.Entities;
using Microsoft.EntityFrameworkCore;

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
                entity.ToTable("documents");  // Setzt den Tabellennamen

                entity.HasKey(e => e.id);  // Setzt den Primärschlüssel


                entity.Property(e => e.title)
                    .IsRequired()
                    .HasMaxLength(200);  // Konfiguriert den "Name"-Spalten

                entity.Property(e => e.author)
                    .IsRequired()
                    .HasMaxLength(80);  // Konfiguriert die "IsComplete"-Spalte

                entity.Property(e => e.date);  // Konfiguriert die "IsComplete"-Spalte
                entity.Property(e => e.contentpath);  // Konfiguriert die "IsComplete"-Spalte
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}