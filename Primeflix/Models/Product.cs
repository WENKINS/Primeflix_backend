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
        [Required]
        public string Title { get; set; }
        [Column("released_date")]
        [Required]
        public DateTime ReleaseDate { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        public int Stock { get; set; }
        [Required]
        public int Rating { get; set; }
        [Required]
        public string Format { get; set; }
        [Column("picture_url")]
        [Required]
        public string PictureUrl { get; set; }
        [Required]
        public double Price { get; set; }
        // Relationships
        public virtual ICollection<Actor>? ActorsMovies { get; set; }
        public virtual ICollection<Director>? DirectorsMovies { get; set; }
        public virtual ICollection<ProductGenre>? ProductGenre { get; set; }
    }   
}
