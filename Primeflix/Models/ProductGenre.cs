using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("genre_product")]
    public class ProductGenre
    {
        [Key]
        [Column("genre_product_id")]
        public int Id { get; set; }

        [Column("genre_id")]
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        [Column("product_id")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
