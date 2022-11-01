using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("product")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int Id { get; set; }
        [Column("original_title")]
        public string Title { get; set; }
        [Column("released_date")]
        public DateTime ReleaseDate { get; set; }
        public int Duration { get; set; }
        public int Stock { get; set; }
        public int Rating { get; set; }
        public string Format { get; set; }
        [Column("picture_url")]
        public string PictureUrl { get; set; }
        public double Price { get; set; }
        // Relationships
        public virtual ICollection<Actor> ActorsMovies { get; set; } 
        public virtual ICollection<ProductGenre> ProductGenre { get; set; }

    }   
}
