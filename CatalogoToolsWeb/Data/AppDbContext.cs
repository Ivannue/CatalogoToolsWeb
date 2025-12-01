using Microsoft.EntityFrameworkCore;
using CatalogoToolsWeb.Models;

namespace CatalogoToolsWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Proveedor> Proveedores { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).HasColumnName("nNombre").HasMaxLength(150);
                entity.Property(e => e.Logo).HasColumnName("iLogo");
                entity.Property(e => e.FechaInsercion).HasColumnName("dFechaInsercion");
                entity.Property(e => e.Activo).HasColumnName("bActivo");
            });
        }
    }
}