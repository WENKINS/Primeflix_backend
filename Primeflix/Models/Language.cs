using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("language")]
    public class Language
    {
        [Key]
        [Column("language_id")]
        public int Id { get; set; }

        public string Name { get; set; }

        // Relationships
        public virtual ICollection<ProductTranslation>? ProductsTranslations { get; set; }
        public virtual ICollection<GenreTranslation>? GenresTranslations { get; set; }
    }
}
