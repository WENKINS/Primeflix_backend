using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("actor_movie")]
    public class Actor
    {
        [Key]
        [Column("actor_id")]
        public int Id { get; set; }
 
        [Column("product_id")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [Column("celebrity_id")]
        public int CelebrityId { get; set; }
        public Celebrity Celebrity { get; set; }

    }
}
