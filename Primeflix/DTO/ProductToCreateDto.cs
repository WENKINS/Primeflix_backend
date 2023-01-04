namespace Primeflix.DTO
{
    public class ProductToCreateDto
    {
        public int? Id { get; set; }
        public string OriginalTitle { get; set; }
        public string EnglishTitle { get; set; }
        public string FrenchTitle { get; set; }
        public string EnglishSummary { get; set; }
        public string FrenchSummary { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Duration { get; set; }
        public int Stock { get; set; }
        public int Rating { get; set; }
        public string PictureUrl { get; set; }
        public double Price { get; set; }
        public int FormatId { get; set; }
        public List<int> ActorsId { get; set; }
        public List<int> DirectorsId { get; set; }
        public List<int> GenresId { get; set; }
    }
}
