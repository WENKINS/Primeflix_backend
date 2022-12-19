using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("order_details")]
    public class OrderDetails
    {
        [Key]
        [Column("order_details_id")]
        public int Id { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }
        public Order Order { get; set; }
        [Column("product_id")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
