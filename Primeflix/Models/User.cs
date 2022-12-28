using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int Id { get; set; }
        [Column("first_name")]
        public string? FirstName { get; set; }
        [Column("last_name")]
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [Column("language_id")]
        public int LanguageId { get; set; }

        public Language Language { get; set; }
        public Cart cart { get; set; }
        [Column("role_id")]
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
