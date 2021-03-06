using System;
using System.Collections.Generic;
using api.entidades;
using api.data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AutoMapper;
using api.models;
using System.Linq;

namespace api.servicos.persistencia
{
    public interface IServicoPersistenciaBase<TPersistenciaModel>
    where TPersistenciaModel : PersistenciaModelBase
    {
        Task<TPersistenciaModel> Criar(TPersistenciaModel novaentidade, bool persistir = false);
        Task<TPersistenciaModel> Alterar(TPersistenciaModel entidadealterada, bool persistir = false);
        Task Excluir(Guid id, bool persistir = false);
        Task<TPersistenciaModel> BuscarPorId(Guid id);
    }

    public abstract class ServicoPersistenciaBase<TEntidade, TPersistenciaModel> : IServicoPersistenciaBase<TPersistenciaModel>
    where TEntidade : EntidadeBase
    where TPersistenciaModel : PersistenciaModelBase
    {
        protected readonly DbContext contexto;
        protected readonly DbSet<TEntidade> entidade_set;
        private readonly IMapper mapeador;

        public ServicoPersistenciaBase(DbContext contexto, IMapper mapeador)
        {
            this.contexto = contexto;
            this.entidade_set = contexto.Set<TEntidade>();
            this.mapeador = mapeador;
        }

        public async Task<TPersistenciaModel> Criar(TPersistenciaModel novomodel, bool persistir = true)
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

            var entidaderetorno = mapeador.Map<TEntidade, TPersistenciaModel>(novaentidade);
            return entidaderetorno;
        }

        public abstract void ValidarCriacao(TPersistenciaModel novomodel);

        public async Task<TPersistenciaModel> Alterar(TPersistenciaModel modelalterado, bool persistir = true)
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

            var entidaderetorno = mapeador.Map<TEntidade, TPersistenciaModel>(entidadeExistente);
            return entidaderetorno;
        }

        public abstract void ValidarAlteracao(TPersistenciaModel novomodel);

        public virtual async Task Excluir(Guid id, bool persistir = true)
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

        public async Task<TPersistenciaModel> BuscarPorId(Guid id)
        {
            var entidadeExistente = await entidade_set.FirstOrDefaultAsync(e => e.Id == id);

            if (entidadeExistente == null) {
                throw new EntidadeNaoExisteException();
            }

            var resultado = mapeador.Map<TPersistenciaModel>(entidadeExistente);
            return resultado;
        }

        protected async Task<IEnumerable<TPersistenciaModel>> EfetuaBusca(Func<DbSet<TEntidade>, IQueryable<TEntidade>> manipulacao_set)
        {
            var setManipulado = manipulacao_set(entidade_set);
            var entidadesExistentes = await setManipulado.ToListAsync();
            var resultado = mapeador.Map<IEnumerable<TPersistenciaModel>>(entidadesExistentes);
            return resultado;
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