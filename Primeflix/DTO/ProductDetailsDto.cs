using Primeflix.Models;

namespace Primeflix.DTO
{
    public class ProductDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Duration { get; set; }
        public int Stock { get; set; }
        public int Rating { get; set; }
        public FormatDto Format { get; set; }
        public string PictureUrl { get; set; }
        public double Price { get; set; }
        public List<CelebrityDto> Directors { get; set; }
        public List<CelebrityDto> Actors { get; set; }
        public List<GenreDto> Genres { get; set; }
    }
}
