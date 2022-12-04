using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("format")]
    public class Format
    {
        [Key]
        [Column("format_id")]
        public int Id { get; set; }
        public string Name { get; set; }
        // Relationships
        public virtual ICollection<Product>? Products { get; set; }
    }
}
