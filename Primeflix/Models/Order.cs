using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("order")]
    public class Order
    {
        [Key]
        [Column("order_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        public User? User { get; set; }
        [Column("status_id")]
        public int StatusId { get; set; }
        public Status? Status { get; set; }
        public DateTime Date {get;set;}
        public float Total { get; set; }
        public virtual ICollection<OrderDetails>? OrderDetails { get; set; }
    }
}
