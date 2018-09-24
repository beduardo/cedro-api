using api.entidades;
using api.models;
using api.data;
using AutoMapper;

namespace api.servicos.persistencia {
    public interface IServicoPersistenciaPrato : IServicoPersistenciaBase<Prato, PratoPersistenciaModel> {

    }

    public class ServicoPersistenciaPrato : ServicoPersistenciaBase<Prato, PratoPersistenciaModel>, IServicoPersistenciaPrato
    {
        public ServicoPersistenciaPrato(ContextoBdAplicacao contexto, IMapper mapeador): base(contexto, mapeador) {

        }

        public override void ValidarAlteracao(PratoPersistenciaModel novomodel)
        {
            throw new System.NotImplementedException();
        }

        public override void ValidarCriacao(PratoPersistenciaModel novomodel)
        {
            throw new System.NotImplementedException();
        }
    }
}