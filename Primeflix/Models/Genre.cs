using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("genre")]
    public class Genre
    {
        [Key]
        [Column("genre_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("genre_name")]
        [Required]
        public string Name { get; set; }

        // Relationships
        public virtual ICollection<ProductGenre>? ProductGenre { get; set; }
    }
}
