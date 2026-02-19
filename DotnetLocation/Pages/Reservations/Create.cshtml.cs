using DotnetLocation.Data;
using DotnetLocation.Elasticsearch.Documents;
using DotnetLocation.Models;
using DotnetLocation.Models.Enums;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetLocation.Pages.Reservations
{
    public class CreateModel : PageModel
    {
        private readonly DotnetLocation.Data.AppDbContext _context;
        private readonly ElasticsearchClient _elastic;

        public CreateModel(DotnetLocation.Data.AppDbContext context, ElasticsearchClient elastic)
        {
            _context = context;
            _elastic = elastic;
        }

        public IActionResult OnGet()
        {
            ViewData["ClientId"] = new SelectList(
             _context.Clients
                 .Select(c => new
                 {
                     c.Id,
                     Display = c.Nom + " " + c.Prenom + " (" + c.Email + ")"
                 })
                 .ToList(),
             "Id",
             "Display"
         );

            ViewData["VehiculeId"] = new SelectList(
                _context.Vehicules
                    .Select(v => new
                    {
                        v.Id,
                        Display = v.Marque + " " + v.Model
                    })
                    .ToList(),
                "Id",
                "Display"
            );

            ViewData["StatusList"] = new SelectList(
                Enum.GetValues(typeof(ReservationStatus))
                    .Cast<ReservationStatus>()
                    .Select(r => new
                    {
                        Value = (int)r,
                        Text = r.ToString()
                    }),
                "Value",
                "Text"
            );


            return Page();
        }

        [BindProperty]
        public Reservation Reservation { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            _context.Reservations.Add(Reservation);
            await _context.SaveChangesAsync();


            var client = await _context.Clients.FindAsync(Reservation.ClientId);
            var vehicule = await _context.Vehicules.FindAsync(Reservation.VehiculeId);
            var cat = await _context.Categories.FindAsync(vehicule?.CategorieId);

            var reseervationEsDoc = new ReservationEsDocument
            {
                ClientEmail = client?.Email ?? string.Empty,
                ClientId = Reservation.ClientId,
                ClientNom = client?.Nom ?? string.Empty,
                ClientPrenom = client?.Prenom ?? string.Empty,
                DateDebut = Reservation.DateDebut,
                DateFin = Reservation.DateFin,
                DateRetourVehicule= Reservation.DateRetourVehicule,
                Id = Reservation.Id,
                Status = Reservation.Status.ToString(),
                VehiculeId = Reservation.VehiculeId,
                VehiculeMarque = vehicule?.Marque ?? string.Empty,
                VehiculeModel = vehicule?.Model ?? string.Empty,
                VehiculeCategorie = cat?.Nom ?? string.Empty
            };

            var esResponse = await _elastic.IndexAsync(reseervationEsDoc, i => i
               .Index("reservations")
               .Id(Reservation.Id) // Important : même ID que SQL
           );

            return RedirectToPage("./Index");
        }
    }
}
