using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("cart_product")]
    public class CartProduct
    {
        [Key]
        [Column("cart_product_id")]
        public int Id { get; set; }

        [Column("cart_id")]
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        [Column("product_id")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
