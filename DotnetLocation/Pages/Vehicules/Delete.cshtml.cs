using DotnetLocation.Data;
using DotnetLocation.Models;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotnetLocation.Pages.Vehicules
{
    public class DeleteModel : PageModel
    {
        private readonly DotnetLocation.Data.AppDbContext _context;
        private readonly ElasticsearchClient _elastic;

        public DeleteModel(DotnetLocation.Data.AppDbContext context ,  ElasticsearchClient elastic)
        {
            _context = context;
            _elastic = elastic;
        }

        [BindProperty]
        public Vehicule Vehicule { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicule = await _context.Vehicules.FirstOrDefaultAsync(m => m.Id == id);

            if (vehicule is not null)
            {
                Vehicule = vehicule;

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

            var vehicule = await _context.Vehicules.FindAsync(id);
            if (vehicule != null)
            {
                Vehicule = vehicule;
                _context.Vehicules.Remove(Vehicule);
                await _context.SaveChangesAsync();

                var esResponse = await _elastic.DeleteAsync(
                index: "vehicules",
                id: id.ToString());
            }

            return RedirectToPage("./Index");
        }
    }
}
