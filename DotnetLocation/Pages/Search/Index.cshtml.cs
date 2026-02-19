using DotnetLocation.Elasticsearch.Documents;
using DotnetLocation.Models;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace DotnetLocation.Pages.Search
{
    public class IndexModel : PageModel
    {
        private readonly ElasticsearchClient _elastic;

        public IndexModel(ElasticsearchClient elastic)
        {
            _elastic = elastic;
        }

        // ✅ Documents Elasticsearch uniquement
        public IList<Client> Clients { get; set; } = new List<Client>();
        public IList<ReservationEsDocument> Reservations { get; set; } = new List<ReservationEsDocument>();
        public IList<VehiculeEsDocument> Vehicules { get; set; } = new List<VehiculeEsDocument>();
        public IList<CategorieEsDocument> Categories { get; set; } = new List<CategorieEsDocument>();

        public async Task<IActionResult> OnGetAsync(string? q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Page();

            // 🔍 CLIENTS
            var clientsResponse = await _elastic.SearchAsync<Client>(s => s
                .Indices("clients")
                .Size(4)
                .Query(qr => qr
                    .MultiMatch(mm => mm
                        .Query(q)
                        .Fields(new[] { "nom", "prenom", "email" })
                    )
                )
            );

            // 🔍 RESERVATIONS
            var reservationsResponse = await _elastic.SearchAsync<ReservationEsDocument>(s => s
                .Indices("reservations")
                .Size(4)
                .Query(qr => qr
                    .QueryString(qs => qs.Query(q))
                )
            );

            // 🔍 VEHICULES
            var vehiculesResponse = await _elastic.SearchAsync<VehiculeEsDocument>(s => s
                .Indices("vehicules")
                .Size(1)
                .Query(qr => qr
                    .MultiMatch(mm => mm
                        .Query(q)
                        .Fields(new[] { "vehicule_Data.marque", "vehicule_Data.modele", "vehicule_Data.immatriculation" })
                    )
                )
            );

            // 🔍 CATEGORIES
            var categoriesResponse = await _elastic.SearchAsync<CategorieEsDocument>(s => s
                .Indices("categories")
                .Size(4)
                .Query(qr => qr
                    .Match(m => m
                        .Field(f => f.Nom)
                        .Query(q)
                    )
                )
            );

            Console.WriteLine(
     JsonSerializer.Serialize(
         clientsResponse.Documents,
         new JsonSerializerOptions
         {
             WriteIndented = true
         }
     )
 );



            Clients = clientsResponse.Documents.ToList();

            Reservations = reservationsResponse.Documents.ToList();
            Vehicules = vehiculesResponse.Documents.ToList();
            Categories = categoriesResponse.Documents.ToList();

          
            return Page();
        }
    }
}
