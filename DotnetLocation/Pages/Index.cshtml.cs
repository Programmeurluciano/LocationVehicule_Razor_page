using DotnetLocation.Data;
using DotnetLocation.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using DotnetLocation.Models.Enums;

namespace DotnetLocation.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        // Statistiques pour les cartes
        public int TotalVehicules { get; set; }
        public int TotalReservations { get; set; }
        public int ReservationsEnCours { get; set; }
        public int TotalClients { get; set; }
        public decimal TotalIncome { get; set; }

        // Top véhicules (par nombre de réservations)
        public List<Vehicule> TopVehicules { get; set; } = new();

        // Réservations récentes
        public List<Reservation> RecentReservations { get; set; } = new();

        // Données pour chart JS
        public List<string> ChartMonths { get; set; } = new();
        public List<decimal> ChartRevenue { get; set; } = new();

        public async Task OnGetAsync()
        {
            TotalVehicules = await _context.Vehicules.CountAsync();
            TotalReservations = await _context.Reservations.CountAsync();

            ReservationsEnCours = await _context.Reservations
                .Where(r => r.Status == ReservationStatus.EnCours)
                .CountAsync();

            TotalClients = await _context.Clients.CountAsync();

            TotalIncome = await _context.Reservations
                .Where(r => r.Status == ReservationStatus.Terminee)
                .SumAsync(r => r.Vehicule.Prix);

            TopVehicules = await _context.Vehicules
                .Include(v => v.Categorie)
                .Include(v => v.Reservations)
                .OrderByDescending(v => v.Reservations.Count)
                .Take(5)
                .ToListAsync();

          
            RecentReservations = await _context.Reservations
                .Include(r => r.Client)
                .Include(r => r.Vehicule)
                .ThenInclude(v => v.Categorie)
                .OrderByDescending(r => r.DateDebut)
                .Take(5)
                .ToListAsync();

            // Chart monthly revenue (exemple: revenus des 6 derniers mois)
            var today = DateTime.Today;
            for (int i = 5; i >= 0; i--)
            {
                var month = new DateTime(today.Year, today.Month, 1).AddMonths(-i);
                ChartMonths.Add(month.ToString("MMM yyyy"));
                var revenue = await _context.Reservations
                    .Where(r => r.DateDebut.Month == month.Month && r.Status == ReservationStatus.Terminee)
                    .SumAsync(r => r.Vehicule.Prix);
                ChartRevenue.Add(revenue);
            }
        }
    }
}
