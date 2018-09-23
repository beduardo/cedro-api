using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.entidades
{
    [Table("Pratos")]
    public class Prato: EntidadeBase {

        public Guid RestauranteId { get; set; }
        public Restaurante Restaurante { get; set; }

        [MaxLength(100)]
        [Required]
        public string Nome { get; set; }

        [Required]
        public decimal Preco { get; set; }
    }
}