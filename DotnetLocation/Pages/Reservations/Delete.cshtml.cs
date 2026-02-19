using DotnetLocation.Data;
using DotnetLocation.Models;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetLocation.Pages.Reservations
{
    public class DeleteModel : PageModel
    {
        private readonly DotnetLocation.Data.AppDbContext _context;
        private readonly ElasticsearchClient _elastic;

        public DeleteModel(DotnetLocation.Data.AppDbContext context, ElasticsearchClient elastic)
        {
            _context = context;
            _elastic = elastic;
        }

        [BindProperty]
        public Reservation Reservation { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FirstOrDefaultAsync(m => m.Id == id);

            if (reservation is not null)
            {
                Reservation = reservation;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                Reservation = reservation;
                _context.Reservations.Remove(Reservation);
                await _context.SaveChangesAsync();

                var esResponse = await _elastic.DeleteAsync(
                   index: "reservations",
                   id: id.ToString()
               );
            }

            return RedirectToPage("./Index");
        }
    }
}
