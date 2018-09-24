using System;
using System.Collections.Generic;
using api.entidades;
using api.data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AutoMapper;
using api.models;

namespace api.servicos.persistencia
{
    public interface IServicoPersistenciaBase<TEntidade, TPersistenciaModel>
    where TEntidade : EntidadeBase
    where TPersistenciaModel : PersistenciaModelBase
    {
        Task Criar(TPersistenciaModel novaentidade, bool persistir = false);
        Task Alterar(TPersistenciaModel entidadealterada, bool persistir = false);
        Task Excluir(Guid id, bool persistir = false);
        TPersistenciaModel BuscarPorId(Guid id);
        IEnumerable<TPersistenciaModel> Buscar();
    }

    public abstract class ServicoPersistenciaBase<TEntidade, TPersistenciaModel> : IServicoPersistenciaBase<TEntidade, TPersistenciaModel>
    where TEntidade : EntidadeBase
    where TPersistenciaModel : PersistenciaModelBase
    {
        private readonly DbContext contexto;
        private readonly DbSet<TEntidade> entidade_set;
        private readonly IMapper mapeador;

        public ServicoPersistenciaBase(DbContext contexto, IMapper mapeador)
        {
            this.contexto = contexto;
            this.entidade_set = contexto.Set<TEntidade>();
            this.mapeador = mapeador;
        }

        public async Task Criar(TPersistenciaModel novomodel, bool persistir = true)
        {

            ValidarCriacao(novomodel);

            var novaentidade = mapeador.Map<TPersistenciaModel, TEntidade>(novomodel);

            novaentidade.Id = Guid.NewGuid();
            novaentidade.DataCriacao = DateTimeOffset.Now;
            novaentidade.DataAlteracao = DateTimeOffset.Now;
            novaentidade.Excluido = false;

            await entidade_set.AddAsync(novaentidade);

            if (persistir)
            {
                await contexto.SaveChangesAsync();
            }
        }

        public abstract void ValidarCriacao(TPersistenciaModel novomodel);

        public async Task Alterar(TPersistenciaModel modelalterado, bool persistir = true)
        {
            ValidarAlteracao(modelalterado);

            //Busca a entidade atual
            var entidadeExistente = await entidade_set.FirstOrDefaultAsync(e => e.Id == modelalterado.Id);

            if (entidadeExistente == null) {
                throw new EntidadeNaoExisteException();
            }

            mapeador.Map<TPersistenciaModel, TEntidade>(modelalterado, entidadeExistente);
            entidadeExistente.DataAlteracao = DateTimeOffset.Now;
            entidadeExistente.Excluido = false;

            if (persistir)
            {
                await contexto.SaveChangesAsync();
            }
        }

        public abstract void ValidarAlteracao(TPersistenciaModel novomodel);

        public async Task Excluir(Guid id, bool persistir = true)
        {
            //Busca a entidade atual
            var entidadeExistente = await entidade_set.FirstOrDefaultAsync(e => e.Id == id);

            if (entidadeExistente == null) {
                throw new EntidadeNaoExisteException();
            }
            
            entidadeExistente.DataAlteracao = DateTimeOffset.Now;
            entidadeExistente.Excluido = true;

            if (persistir)
            {
                await contexto.SaveChangesAsync();
            }
        }

        public TPersistenciaModel BuscarPorId(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TPersistenciaModel> Buscar()
        {
            throw new NotImplementedException();
        }

    }

    public class EntidadeNaoExisteException : Exception
    {
        public EntidadeNaoExisteException() : base("Entidade não encontrada") { }
    }

    public class ValidacaoPersistenciaException : Exception
    {
        public ErroValidacaoPropriedade[] Erros { get; set; }

        public ValidacaoPersistenciaException(ErroValidacaoPropriedade[] Erros) : base("Entidade inválida")
        {
            this.Erros = Erros;
        }
    }

    public class ErroValidacaoPropriedade
    {
        public ErroValidacaoPropriedade(string Propriedade, string[] Erros)
        {
            this.Propriedade = Propriedade;
            this.Erros = Erros;
        }
        public string Propriedade { get; set; }
        public string[] Erros { get; set; }
    }
}