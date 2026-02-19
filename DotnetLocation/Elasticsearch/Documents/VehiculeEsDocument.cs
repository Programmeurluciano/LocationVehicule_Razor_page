namespace DotnetLocation.Elasticsearch.Documents
{
  

    public class VehiculeEsDocument
    {
        public VehiculeData Vehicule_Data { get; set; } = null!;
        public List<string> Images { get; set; } = new();
    }

    public class VehiculeData
    {
        public int Id { get; set; }

        // Catégorie
        public int CategorieId { get; set; }
        public string CategorieNom { get; set; } = string.Empty;

        // Infos véhicule
        public string Marque { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Annee { get; set; }
        public string Transmission { get; set; } = string.Empty;
        public string Carburant { get; set; } = string.Empty;

        // Prix
        public decimal Prix { get; set; }
        public decimal Caution { get; set; }
        public decimal Penalite { get; set; }

        // Disponibilité
        public bool Disponibilite { get; set; }
    }

}
