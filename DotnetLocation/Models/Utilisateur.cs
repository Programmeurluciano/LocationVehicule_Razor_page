using System.ComponentModel.DataAnnotations;

namespace DotnetLocation.Models
{
    
    public class Utilisateur
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string MotDePasseHash { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Role { get; set; } = "Admin";
        // Admin, Manager, etc.
        public bool Actif { get; set; } = true;
    }

}
