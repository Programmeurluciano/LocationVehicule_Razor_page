using Microsoft.EntityFrameworkCore;
using DotnetLocation.Models;


namespace DotnetLocation.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)
        {
        }

        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Vehicule> Vehicules { get; set; }
        public DbSet<VehiculeImage> VehiculeImages { get; set; }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<Utilisateur> Utilisateurs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Email client unique
            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Email)
                .IsUnique();

            // Email user unique
            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Categorie -> Vehicule
            modelBuilder.Entity<Categorie>()
                .HasMany(c => c.Vehicules)
                .WithOne(v => v.Categorie)
                .HasForeignKey(v => v.CategorieId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicule -> Images
            modelBuilder.Entity<Vehicule>()
                .HasMany(v => v.Images)
                .WithOne(i => i.Vehicule)
                .HasForeignKey(i => i.VehiculeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Client -> Reservation
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Reservations)
                .WithOne(r => r.Client)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicule -> Reservation
            modelBuilder.Entity<Vehicule>()
                .HasMany<Reservation>()
                .WithOne(r => r.Vehicule)
                .HasForeignKey(r => r.VehiculeId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
