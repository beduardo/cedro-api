using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.entidades
{
    public class EntidadeBase {

        //Propriedades padrão
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTimeOffset DataCriacao { get; set; }

        [Required]
        public DateTimeOffset DataAlteracao { get; set; }

        [Required]
        public bool Excluido { get; set; }
    }
}