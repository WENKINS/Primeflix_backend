namespace Primeflix.DTO
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Stock { get; set; }
        public int Rating { get; set; }
        public string Format { get; set; }
        public string PictureUrl { get; set; }
        public double Price { get; set; }
        public List<GenreDto> Genres { get; set; }
    }
}
