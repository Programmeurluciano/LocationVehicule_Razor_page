using System;
using Elastic.Clients.Elasticsearch.Mapping;

namespace DotnetLocation.Elasticsearch.Documents
{
    public class ReservationEsDocument
    {
        public int Id { get; set; }
         
        public int ClientId { get; set; }

        public string ClientNom { get; set; } = string.Empty;

        public string ClientPrenom { get; set; } = string.Empty;

        public string ClientEmail { get; set; } = string.Empty;

        public int VehiculeId { get; set; }

        public string VehiculeMarque { get; set; } = string.Empty;

        public string VehiculeModel { get; set; } = string.Empty;

        public string VehiculeCategorie { get; set; } = string.Empty;

        public DateTime DateDebut { get; set; }

        public DateTime DateFin { get; set; }

        public DateTime? DateRetourVehicule { get; set; }

        public string Status { get; set; } = string.Empty;
    }


}
