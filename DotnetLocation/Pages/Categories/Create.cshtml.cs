using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DotnetLocation.Data;
using DotnetLocation.Models;
using Elastic.Clients.Elasticsearch;


namespace DotnetLocation.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly DotnetLocation.Data.AppDbContext _context;
        private readonly ElasticsearchClient _elastic;


        public CreateModel(DotnetLocation.Data.AppDbContext context , ElasticsearchClient elastic)
        {
            _context = context;
            _elastic = elastic;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Categorie Categorie { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Categories.Add(Categorie);
            await _context.SaveChangesAsync();

            var esResponse = await _elastic.IndexAsync(Categorie, i => i
               .Index("categories")
               .Id(Categorie.Id) // Important : même ID que SQL
           );

            // 3️⃣ Si Elasticsearch échoue (log mais ne bloque pas)
            if (!esResponse.IsValidResponse)
            {
                // Optionnel : logger l'erreur
                Console.WriteLine("---------------------TSY METY---------------");

                Console.WriteLine(esResponse.DebugInformation);
            } else
            {
                Console.WriteLine("---------------------METY---------------");
            }

                return RedirectToPage("./Index");
        }
    }
}
