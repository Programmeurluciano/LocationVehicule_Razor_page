using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DotnetLocation.Data;
using DotnetLocation.Models;

namespace DotnetLocation.Pages.Vehicules
{
    public class CreateModel : PageModel
    {
        private readonly DotnetLocation.Data.AppDbContext _context;

        public CreateModel(DotnetLocation.Data.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["CategorieId"] = new SelectList(_context.Categories, "Id", "Nom");
            return Page();
        }

        [BindProperty]
        public Vehicule Vehicule { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Vehicules.Add(Vehicule);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
