using DotnetLocation.Data;
using DotnetLocation.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.MachineLearning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetLocation.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly DotnetLocation.Data.AppDbContext _context;
        private readonly ElasticsearchClient _elastic;

        public EditModel(DotnetLocation.Data.AppDbContext context , ElasticsearchClient elastic)
        {
            _context = context; 
            _elastic = elastic;
        }

        [BindProperty]
        public Categorie Categorie { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorie =  await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
            if (categorie == null)
            {
                return NotFound();
            }
            Categorie = categorie;
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

            _context.Attach(Categorie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                var esResponse = await _elastic.UpdateAsync<Categorie, Categorie>(
                    index: "categories",
                    id: Categorie.Id.ToString(),
                    u => u.Doc(Categorie).DocAsUpsert(true)
                );

                if (!esResponse.IsValidResponse)
                {
                    Console.WriteLine("❌ Erreur update Elasticsearch");
                    Console.WriteLine(esResponse.DebugInformation);
                }
                else
                {
                    Console.WriteLine("✅ Update Elasticsearch OK");
                }

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategorieExists(Categorie.Id))
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

        private bool CategorieExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
