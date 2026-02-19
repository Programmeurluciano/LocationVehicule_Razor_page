using DotnetLocation.Elasticsearch.Documents;
using DotnetLocation.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DotnetLocation.Pages.Reservations.Search
{
    public class IndexModel : PageModel
    {
        private readonly ElasticsearchClient _elastic;
        private readonly DotnetLocation.Data.AppDbContext _context;

        public IndexModel(ElasticsearchClient elastic , DotnetLocation.Data.AppDbContext context)
        {
            _elastic = elastic;
            _context = context;
        }

        public IReadOnlyCollection<ReservationEsDocument> Reservations { get; set; }
            = Array.Empty<ReservationEsDocument>();

        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 20;

        public long TotalResults { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Categorie { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? DateDebut { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? DateFin { get; set; }

        public IList<Categorie> CategorieList { get; set; } = default!;
        public async Task OnGetAsync()
        {
            CategorieList = await _context.Categories.ToListAsync();
            // Pagination sécurisée
            PageNumber = Page > 0 ? Page : 1;
            PageSize = Math.Clamp(PageSize, 1, 100);

            var mustQueries = new List<Query>();
            var filterQueries = new List<Query>();

            // 🔹 Recherche textuelle fuzzy
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                mustQueries.Add(new MultiMatchQuery
                {
                    Query = SearchTerm,
                    Fuzziness = new Fuzziness("AUTO"),
                    Operator = Operator.Or,
                    Fields = new[]
                    {
                        Infer.Field<ReservationEsDocument>(p => p.ClientNom),
                        Infer.Field<ReservationEsDocument>(p => p.ClientPrenom),
                        Infer.Field<ReservationEsDocument>(p => p.VehiculeMarque),
                        Infer.Field<ReservationEsDocument>(p => p.VehiculeModel),
                        Infer.Field<ReservationEsDocument>(p => p.Status),
                        Infer.Field<ReservationEsDocument>(p => p.VehiculeCategorie),

                    }
                });
            }

            // ⚙️ Filtres exacts
            if (!string.IsNullOrWhiteSpace(Status))
            {
                filterQueries.Add(new TermQuery
                {
                    Field = Infer.Field("status.keyword"),
                    Value = Status
                });
            }

            if (!string.IsNullOrWhiteSpace(Categorie))
            {
                filterQueries.Add(new TermQuery
                {
                    Field = Infer.Field<ReservationEsDocument>(p => p.VehiculeCategorie),
                    Value = Categorie
                });
            }

            // 📅 Filtre date
            if (DateDebut != null || DateFin != null)
            {
                filterQueries.Add(new DateRangeQuery
                {
                    Field = Infer.Field<ReservationEsDocument>(p => p.DateDebut),
                    Gte = DateDebut,
                    Lte = DateFin
                });
            }

            // 🔎 Construction finale de la query
            Query finalQuery;

            if (!mustQueries.Any() && !filterQueries.Any())
            {
                // Aucune recherche, aucun filtre → tout retourner
                finalQuery = new MatchAllQuery();
            }
            else
            {
                finalQuery = new BoolQuery
                {
                    Must = mustQueries.Any() ? mustQueries : null,
                    Filter = filterQueries.Any() ? filterQueries : null
                };
            }

            // 🔹 Exécution de la recherche
            var response = await _elastic.SearchAsync<ReservationEsDocument>(s => s
                .Indices("reservations")
                .From((PageNumber - 1) * PageSize)
                .Size(PageSize)
                .TrackTotalHits(true) // important pour TotalResults correct
                .Query(finalQuery)
            );

            Console.WriteLine(response);

            Reservations = response.Documents;
            TotalResults = response.Total;
        }
    }
}

