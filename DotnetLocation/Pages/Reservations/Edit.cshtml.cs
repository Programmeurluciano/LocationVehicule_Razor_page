using DotnetLocation.Data;
using DotnetLocation.Models;
using DotnetLocation.Models.Enums;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetLocation.Pages.Reservations
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ElasticsearchClient _elastic;

        public EditModel(AppDbContext context, ElasticsearchClient elastic)
        {
            _context = context;
            _elastic = elastic;
        }

        [BindProperty]
        public Reservation Reservation { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Reservation == null)
                return NotFound();

            // 🔹 CLIENT : Nom + Prénom + Email
            ViewData["ClientId"] = new SelectList(
                _context.Clients.Select(c => new
                {
                    c.Id,
                    Display = c.Nom + " " + c.Prenom + " (" + c.Email + ")"
                }),
                "Id",
                "Display",
                Reservation.ClientId
            );

            // 🔹 VEHICULE : Marque + Modèle
            ViewData["VehiculeId"] = new SelectList(
                _context.Vehicules.Select(v => new
                {
                    v.Id,
                    Display = v.Marque + " " + v.Model
                }),
                "Id",
                "Display",
                Reservation.VehiculeId
            );

            // 🔹 STATUS : Enum lisible
            ViewData["StatusList"] = new SelectList(
                Enum.GetValues(typeof(ReservationStatus))
                    .Cast<ReservationStatus>()
                    .Select(s => new
                    {
                        Value = (int)s,
                        Text = s.ToString()
                    }),
                "Value",
                "Text",
                (int)Reservation.Status
            );


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Attach(Reservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // 🔒 ELASTICSEARCH : INCHANGÉ
                var esResponse = await _elastic.UpdateAsync<Reservation, Reservation>(
                    index: "reservations",
                    id: Reservation.Id.ToString(),
                    u => u.Doc(Reservation).DocAsUpsert(true)
                );
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(Reservation.Id))
                    return NotFound();

                throw;
            }

            return RedirectToPage("./Index");
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
