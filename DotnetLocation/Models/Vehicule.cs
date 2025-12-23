using DotnetLocation.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotnetLocation.Models
{
    public class Vehicule
    {
        [Key]
        public int Id { get; set; }

        // Catégorie
        [Required]
        public int CategorieId { get; set; }
        public Categorie Categorie { get; set; } = null!;

        // Infos véhicule
        [Required]
        [MaxLength(100)]
        public string Marque { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        [Range(1900, 2100)]
        public int Annee { get; set; }

        [Required]
        [MaxLength(50)]
        public string Transmission { get; set; } = string.Empty;
        // Ex: Manuelle, Automatique

        [Required]
        [MaxLength(50)]
        public string Carburant { get; set; } = string.Empty;
        // Ex: Essence, Diesel, Électrique

        [Range(0, double.MaxValue)]
        public decimal Prix { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Caution { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Penalite { get; set; }

        //  Disponibilité
        public bool Disponibilite { get; set; }

        //  Images
        public ICollection<VehiculeImage> Images { get; set; } = new List<VehiculeImage>();
    }
}