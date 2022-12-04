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

        [Column("picture_url")]
        [Required]
        public string PictureUrl { get; set; }

        [Required]
        public double Price { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("updated_by")]
        public string? UpdatedBy { get; set; }

        // Relationships

        [Column("format_id")]
        public int FormatId { get; set; }

        public Format Format { get; set; }

        public virtual ICollection<Actor>? ActorsMovies { get; set; }

        public virtual ICollection<Director>? DirectorsMovies { get; set; }

        public virtual ICollection<ProductGenre>? ProductGenre { get; set; }

        public virtual ICollection<ProductTranslation>? ProductsTranslations { get; set; }
    }   
}
