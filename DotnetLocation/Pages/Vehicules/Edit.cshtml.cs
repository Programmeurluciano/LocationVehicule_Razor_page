using DotnetLocation.Data;
using DotnetLocation.Models;
using DotnetLocation.Elasticsearch.Documents;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetLocation.Pages.Vehicules
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ElasticsearchClient _elastic;

        public EditModel(AppDbContext context, ElasticsearchClient elastic)
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

            var vehicule = await _context.Vehicules
                .Include(v => v.Images)
                .Include(v => v.Categorie)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (vehicule == null)
            {
                return NotFound();
            }

            Vehicule = vehicule;
            ViewData["CategorieId"] = new SelectList(_context.Categories, "Id", "Nom");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Attacher l'entité et marquer comme modifiée
            _context.Attach(Vehicule).State = EntityState.Modified;

            try
            {
                // 1️⃣ Mise à jour DB
                await _context.SaveChangesAsync();

                // 2️⃣ Préparer document ES
                var esDocument = new VehiculeEsDocument
                {
                    Vehicule_Data = new VehiculeData
                    {
                        Id = Vehicule.Id,
                        CategorieId = Vehicule.CategorieId,
                        CategorieNom = Vehicule.Categorie?.Nom ?? string.Empty,

                        Marque = Vehicule.Marque,
                        Model = Vehicule.Model,
                        Annee = Vehicule.Annee,
                        Transmission = Vehicule.Transmission,
                        Carburant = Vehicule.Carburant,

                        Prix = Vehicule.Prix,
                        Caution = Vehicule.Caution,
                        Penalite = Vehicule.Penalite,

                        Disponibilite = Vehicule.Disponibilite
                    },
                    Images = Vehicule.Images.Select(i => i.ImageUrl).ToList()
                };

                // 3️⃣ Update ES (ou insert si inexistant)
                var esResponse = await _elastic.UpdateAsync<VehiculeEsDocument, VehiculeEsDocument>(
                    index: "vehicules",
                    id: Vehicule.Id.ToString(),
                    u => u.Doc(esDocument).DocAsUpsert(true)
                );

                if (!esResponse.IsValidResponse)
                {
                    Console.WriteLine("❌ Erreur update Elasticsearch pour le véhicule Id: " + Vehicule.Id);
                    Console.WriteLine(esResponse.DebugInformation);
                }
                else
                {
                    Console.WriteLine("✅ Véhicule mis à jour dans Elasticsearch, Id: " + Vehicule.Id);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehiculeExists(Vehicule.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("./Index");
        }

        private bool VehiculeExists(int id)
        {
            return _context.Vehicules.Any(e => e.Id == id);
        }
    }
}
