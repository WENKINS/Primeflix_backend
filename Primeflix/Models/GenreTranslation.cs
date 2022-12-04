using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("genre_translation")]
    public class GenreTranslation
    {
        [Key]
        [Column("genre_translation_id")]
        public int Id { get; set; }

        [Column("genre_id")]
        public int GenreId { get; set; }

        public Genre Genre { get; set; }

        [Column("language_id")]
        public int LanguageId { get; set; }

        public Language Language { get; set; }

        [Column("genre_translation")]
        public string Translation { get; set; }
    }
}
