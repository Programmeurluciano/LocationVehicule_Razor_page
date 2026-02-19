using DotnetLocation.Elasticsearch.Documents;
using DotnetLocation.Models;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetLocation.Pages.Clients.Search
{
    public class IndexModel : PageModel
    {
        private readonly ElasticsearchClient _elastic;

        public IndexModel(ElasticsearchClient elastic)
        {
            _elastic = elastic;
        }

        // Résultats de recherche
        public IList<Client> Clients { get; set; } = new List<Client>();

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20; // nombre d'éléments par page
        public long TotalResults { get; set; }

        // Champ de recherche
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;

        public async Task OnGetAsync()
        {
            PageNumber = Page;

            var response = await _elastic.SearchAsync<Client>(s => s
                .Indices("clients")
                .From((PageNumber - 1) * PageSize)
                .Size(PageSize)
                 .Query(qr => qr
                    .MultiMatch(mm => mm
                        .Query(SearchTerm)
                        .Fields(new[] { "nom", "prenom", "email", "contact" })
                    )
                )
            );

            Clients = response.Hits.Select(h => h.Source!).ToList();
            TotalResults = response.Total;
        }
    }
}
