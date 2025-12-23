using System.ComponentModel.DataAnnotations;

namespace DotnetLocation.Models
{
    public class VehiculeImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public bool EstPrincipale { get; set; }

        //  Véhicule
        public int VehiculeId { get; set; }
        public Vehicule Vehicule { get; set; } = null!;
    }
}