using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DotnetLocation.Data;
using DotnetLocation.Models;

namespace DotnetLocation.Pages.Vehicules
{
    public class IndexModel : PageModel
    {
        private readonly DotnetLocation.Data.AppDbContext _context;

        public IndexModel(DotnetLocation.Data.AppDbContext context)
        {
            _context = context;
        }

        public IList<Vehicule> Vehicule { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Vehicule = await _context.Vehicules
                .Include(v => v.Categorie).ToListAsync();
        }
    }
}
