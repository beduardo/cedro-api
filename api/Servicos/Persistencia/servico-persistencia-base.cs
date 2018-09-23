using System;
using api.entidades;

namespace api.servicos.persistencia {
    public class ServicoPersistenciaBase<TEntidade> where TEntidade : EntidadeBase
    {
        public void PreparaCriar(TEntidade entidade, Guid id = default(Guid))
        {       
            if (id == default(Guid)) {
                id = Guid.NewGuid();
            }
            entidade.Id = id;
            entidade.DataCriacao = DateTimeOffset.Now;
            entidade.DataAlteracao = DateTimeOffset.Now;
            entidade.Excluido = false;
        }
        
        public void PreparaAlterar(TEntidade entidade)
        {
            entidade.DataAlteracao = DateTimeOffset.Now;
            entidade.Excluido = false; //Caso o registro tenha sido excluído, ao alterá-lo ele será restaurado
        }
        
        public void PreparaExcluir(TEntidade entidade)
        {
            entidade.DataAlteracao = DateTimeOffset.Now;
            entidade.Excluido = true; //Caso o registro tenha sido excluído, ao alterá-lo ele será restaurado
        }
    }
}