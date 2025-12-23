using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DotnetLocation.Data;
using DotnetLocation.Models;

namespace DotnetLocation.Pages.Categories
{
    public class DetailsModel : PageModel
    {
        private readonly DotnetLocation.Data.AppDbContext _context;

        public DetailsModel(DotnetLocation.Data.AppDbContext context)
        {
            _context = context;
        }

        public Categorie Categorie { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorie = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

            if (categorie is not null)
            {
                Categorie = categorie;

                return Page();
            }

            return NotFound();
        }
    }
}
