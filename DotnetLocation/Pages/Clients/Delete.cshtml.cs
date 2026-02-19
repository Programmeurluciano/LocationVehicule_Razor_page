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

namespace DotnetLocation.Pages.Clients
{
    public class DeleteModel : PageModel
    {
        private readonly DotnetLocation.Data.AppDbContext _context;
        private readonly ElasticsearchClient _elastic;

        public DeleteModel(DotnetLocation.Data.AppDbContext context ,ElasticsearchClient elastic)
        {
            _context = context;
            _elastic = elastic;
        }

        [BindProperty]
        public Client Client { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FirstOrDefaultAsync(m => m.Id == id);

            if (client is not null)
            {
                Client = client;

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

            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                Client = client;
                _context.Clients.Remove(Client);
                await _context.SaveChangesAsync();


                var esResponse = await _elastic.DeleteAsync(
                index: "clients",
                id: id.ToString()
            );

            }

            return RedirectToPage("./Index");
        }
    }
}
