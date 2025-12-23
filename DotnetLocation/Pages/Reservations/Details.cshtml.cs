using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DotnetLocation.Data;
using DotnetLocation.Models;

namespace DotnetLocation.Pages.Reservations
{
    public class DetailsModel : PageModel
    {
        private readonly DotnetLocation.Data.AppDbContext _context;

        public DetailsModel(DotnetLocation.Data.AppDbContext context)
        {
            _context = context;
        }

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
    }
}
