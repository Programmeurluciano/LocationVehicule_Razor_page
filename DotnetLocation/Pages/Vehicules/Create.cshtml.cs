using DotnetLocation.Data;
using DotnetLocation.Models;
using Elastic.Clients.Elasticsearch;
using DotnetLocation.Elasticsearch.Documents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetLocation.Pages.Vehicules
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ElasticsearchClient _elastic;

        public CreateModel(AppDbContext context , ElasticsearchClient elastic)
        {
            _context = context;
            _elastic = elastic; 
        }

        public IActionResult OnGet()
        {
            ViewData["CategorieId"] = new SelectList(_context.Categories, "Id", "Nom");
            return Page();
        }

        [BindProperty]
        public Vehicule Vehicule { get; set; } = new();

        [BindProperty]
        public Categorie Categorie  { get; set; } = new();

        [BindProperty]
        public List<IFormFile> Images { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {

            // 1️⃣ Sauvegarde véhicule
            _context.Vehicules.Add(Vehicule);
            await _context.SaveChangesAsync();

            // 2️⃣ Dossier images
            var uploadPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/images/vehicules"
            );
            Directory.CreateDirectory(uploadPath);

            // 3️⃣ Sauvegarde images
            if (Images != null && Images.Count > 0)
            {
                bool firstImage = true;

                foreach (var image in Images)
                {
                    if (image.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(uploadPath, fileName);

                        using var stream = new FileStream(filePath, FileMode.Create);
                        await image.CopyToAsync(stream);

                        Vehicule.Images.Add(new VehiculeImage
                        {
                            ImageUrl = "/images/vehicules/" + fileName,
                            EstPrincipale = firstImage,
                            VehiculeId = Vehicule.Id
                        });

                        firstImage = false;
                    }
                }

                await _context.SaveChangesAsync();
            }

            var categorie = await _context.Categories.FindAsync(Vehicule.CategorieId);


            var esDocument = new VehiculeEsDocument
            {
                Vehicule_Data = new VehiculeData
                {
                    Id = Vehicule.Id,
                    CategorieId = Vehicule.CategorieId,
                    CategorieNom = categorie?.Nom ?? string.Empty,
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

                Images = Vehicule.Images
            .Select(i => i.ImageUrl)
            .ToList()
            };

            var esResponse = await _elastic.IndexAsync(
               esDocument,
               i => i
                   .Index("vehicules")
                   .Id(Vehicule.Id.ToString())
           );


            return RedirectToPage("./Index");
        }

    }
}
