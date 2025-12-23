using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotnetLocation.Models
{
    public class Categorie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<Vehicule> Vehicules { get; set; } = new List<Vehicule>();
    }
}