using DotnetLocation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace DotnetLocation.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
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
                .HasMany(c => c.Reservations)
                .WithOne(r => r.Vehicule)
                .HasForeignKey(r => r.VehiculeId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
