using DotnetLocation.Elasticsearch.Documents;
using DotnetLocation.Models;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetLocation.Pages.Vehicules.Search
{
    public class IndexModel : PageModel
    {
        private readonly ElasticsearchClient _elastic;
        private readonly DotnetLocation.Data.AppDbContext _context;

        public IndexModel(ElasticsearchClient elastic, DotnetLocation.Data.AppDbContext context)
        {
            _elastic = elastic;
            _context = context;
        }

        public IList<VehiculeEsDocument> Vehicules { get; set; } = new List<VehiculeEsDocument>();
        public IList<Categorie> Categories { get; set; } = default!;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public long TotalResults { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public int PageNumberParam { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int? CategorieId { get; set; }

        public async Task OnGetAsync()
        {
            PageNumber = PageNumberParam;

            var mustQueries = new List<Elastic.Clients.Elasticsearch.QueryDsl.Query>();
            var filterQueries = new List<Elastic.Clients.Elasticsearch.QueryDsl.Query>();

            // 🔍 Recherche texte
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                mustQueries.Add(new Elastic.Clients.Elasticsearch.QueryDsl.MultiMatchQuery
                {
                    Query = SearchTerm,
                    Fields = new[]
                    {
                "vehicule_Data.marque",
                "vehicule_Data.model",
                "vehicule_Data.transmission",
                "vehicule_Data.carburant",
                "vehicule_Data.categorieNom"
            },
                    Fuzziness = "AUTO" // ⭐ recommandé pour Jane ≈ Jan, Jame, etc.
                });
            }
            else
            {
                mustQueries.Add(new Elastic.Clients.Elasticsearch.QueryDsl.MatchAllQuery());
            }

            // 🎯 Filtre catégorie
            if (CategorieId.HasValue)
            {
                filterQueries.Add(new Elastic.Clients.Elasticsearch.QueryDsl.TermQuery
                {
                    Field = "vehicule_Data.categorieId",
                    Value = CategorieId.Value
                });
            }

            // 🔎 Recherche Elasticsearch
            var response = await _elastic.SearchAsync<VehiculeEsDocument>(s => s
                .Indices("vehicules")
                .From((PageNumber - 1) * PageSize)
                .Size(PageSize)
                .Query(q => q.Bool(b => b
                    .Must(mustQueries)
                    .Filter(filterQueries)
                ))
            );

            // 📦 Résultats
            Categories = _context.Categories.ToList();
            Vehicules = response.Hits.Select(h => h.Source!).ToList();
            TotalResults = response.Total;
        }


    }
}
