using DotnetLocation.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace DotnetLocation.Models
{
    
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        //  Client
        [Required]
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        //  Véhicule
        [Required]
        public int VehiculeId { get; set; }
        public Vehicule Vehicule { get; set; } = null!;

        //  Dates
        [Required]
        public DateTime DateDebut { get; set; }

        [Required]
        public DateTime DateFin { get; set; }

        //  Date réelle de retour
        public DateTime? DateRetourVehicule { get; set; }

        //  Statut
        [Required]
        public ReservationStatus Status { get; set; } = ReservationStatus.EnCours;


    }

}
