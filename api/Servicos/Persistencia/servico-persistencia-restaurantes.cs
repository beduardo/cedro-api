using api.entidades;
using api.models;
using api.data;
using AutoMapper;

namespace api.servicos.persistencia {
    public interface IServicoPersistenciaRestaurante : IServicoPersistenciaBase<Restaurante, RestaurantePersistenciaModel> {

    }

    public class ServicoPersistenciaRestaurante : ServicoPersistenciaBase<Restaurante, RestaurantePersistenciaModel>, IServicoPersistenciaRestaurante
    {
        public ServicoPersistenciaRestaurante(ContextoBdAplicacao contexto, IMapper mapeador): base(contexto, mapeador) {

        }
        
        public override void ValidarAlteracao(RestaurantePersistenciaModel novomodel)
        {
            throw new System.NotImplementedException();
        }

        public override void ValidarCriacao(RestaurantePersistenciaModel novomodel)
        {
            throw new System.NotImplementedException();
        }
    }
}