using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    public class Product
    {
        [Column("product_id")]
        public int Id { get; set; }
        [Column("original_title")]
        public string Title { get; set; }
        [Column("released_date")]
        public DateTime ReleaseDate { get; set; }
        public TimeSpan Duration { get; set; }
        public int Stock { get; set; }
        public int Rating { get; set; }
        public string Format { get; set; }
        public string Description { get; set; }
    }   
}
