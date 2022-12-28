using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("Role")]
    public class Role
    {
        [Key]
        [Column("role_id")]
        public int Id { get; set; }

        public string Name { get; set; }

        // Relationships
        public virtual ICollection<User>? Users { get; set; }
    }
}
