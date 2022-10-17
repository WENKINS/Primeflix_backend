using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("genre")]
    public class Genre
    {
        [Key]
        [Column("genre_id")]
        public int Id { get; set; }
        public string Name { get; set; }

        // Relationships
        public virtual ICollection<ProductGenre> ProductGenre { get; set; }
    }
}
