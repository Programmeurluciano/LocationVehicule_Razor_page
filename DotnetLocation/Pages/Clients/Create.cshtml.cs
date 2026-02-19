using DotnetLocation.Data;
using DotnetLocation.Models;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetLocation.Pages.Clients
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
        public Client Client { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Clients.Add(Client);
            await _context.SaveChangesAsync();

            var esResponse = await _elastic.IndexAsync(Client, i => i
               .Index("clients")
               .Id(Client.Id)
           );

            return RedirectToPage("./Index");
        }
    }
}
