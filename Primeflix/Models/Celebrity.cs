using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("celebrity")]
    public class Celebrity
    {
        [Key]
        [Column("celebrity_id")]
        public int Id { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        // Relationships
        public virtual ICollection<Actor>? ActorsMovies { get; set; }
        public virtual ICollection<Director>? DirectorsMovies { get; set; }
    }
}
