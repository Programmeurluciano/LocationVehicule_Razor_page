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

namespace DotnetLocation.Pages.Categories
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorie = await _context.Categories.FindAsync(id);
            if (categorie != null)
            {
                Categorie = categorie;
                _context.Categories.Remove(Categorie);
                await _context.SaveChangesAsync();
            }

            var esResponse = await _elastic.DeleteAsync(
                index: "categories",           
                id: id.ToString()               
            );


            if (!esResponse.IsValidResponse)
            {
                // Optionnel : logger l'erreur
                Console.WriteLine("---------------------TSY METY---------------");

                Console.WriteLine(esResponse.DebugInformation);
            }
            else
            {
                Console.WriteLine("---------------------METY---------------");
            }

            return RedirectToPage("./Index");
        }
    }
}
