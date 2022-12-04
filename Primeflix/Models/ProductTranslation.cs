using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("product_translation")]
    public class ProductTranslation
    {
        [Key]
        [Column("product_translation_id")]
        public int Id { get; set; }

        [Column("language_id")]
        public int LanguageId { get; set; }

        public Language Language { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}
