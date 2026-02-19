using Elastic.Clients.Elasticsearch.Mapping;

namespace DotnetLocation.Elasticsearch.Documents
{
    public class CategorieEsDocument
    {
        public int Id { get; set; }

        public string Nom { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
