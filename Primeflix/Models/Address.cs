using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("address")]
    public class Address
    {
        [Key]
        [Column("address_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Street { get; set; }
        [Column("house_number")]
        public string Number { get; set; }
        [Column("postal_code")]
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
