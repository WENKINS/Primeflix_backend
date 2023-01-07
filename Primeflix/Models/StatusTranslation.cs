using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primeflix.Models
{
    public class StatusTranslation
    {
        [Key]
        [Column("status_translation_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public int LanguageId { get; set; }
        public Language Language { get; set; }
        public string Name { get; set; }
    }
}
