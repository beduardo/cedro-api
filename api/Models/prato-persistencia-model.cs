using System;

namespace api.models
{
    public class PratoPersistenciaModel : PersistenciaModelBase
    {
        public Guid RestauranteId { get; set; }

        public string Nome { get; set; }

        public decimal Preco { get; set; }
    }
}