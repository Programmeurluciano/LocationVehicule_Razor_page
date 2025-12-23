using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotnetLocation.Data;
using DotnetLocation.Models;

namespace DotnetLocation.Pages.Vehicules
{
    public class EditModel : PageModel
    {
        private readonly DotnetLocation.Data.AppDbContext _context;

        public EditModel(DotnetLocation.Data.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Vehicule Vehicule { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicule =  await _context.Vehicules.FirstOrDefaultAsync(m => m.Id == id);
            if (vehicule == null)
            {
                return NotFound();
            }
            Vehicule = vehicule;
           ViewData["CategorieId"] = new SelectList(_context.Categories, "Id", "Nom");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Vehicule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehiculeExists(Vehicule.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool VehiculeExists(int id)
        {
            return _context.Vehicules.Any(e => e.Id == id);
        }
    }
}
