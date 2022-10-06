using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("actor_movie")]
    public class Actor
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int CelebrityId { get; set; }
        public Celebrity Celebrity { get; set; }

    }
}
