using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.entidades
{
    [Table("Restaurantes")]
    public class Restaurante: EntidadeBase {

        [MaxLength(100)]
        [Required]
        public string Nome { get; set; }
    }
}