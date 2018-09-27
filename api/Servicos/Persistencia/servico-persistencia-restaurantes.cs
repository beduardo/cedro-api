using api.entidades;
using api.models;
using api.data;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace api.servicos.persistencia
{
    public interface IServicoPersistenciaRestaurante : IServicoPersistenciaBase<RestaurantePersistenciaModel>
    {
        Task<IEnumerable<RestaurantePersistenciaModel>> Buscar(string filtro);
    }

    public class ServicoPersistenciaRestaurante : ServicoPersistenciaBase<Restaurante, RestaurantePersistenciaModel>, IServicoPersistenciaRestaurante
    {
        public ServicoPersistenciaRestaurante(ContextoBdAplicacao contexto, IMapper mapeador) : base(contexto, mapeador)
        {

        }

        public Task<IEnumerable<RestaurantePersistenciaModel>> Buscar(string filtro = null)
        {
            Func<IQueryable<Restaurante>, IQueryable<Restaurante>> geraFiltro = (query) => {
                var palavras = filtro.ToLower().Split(" ").Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim());

                foreach(var palavra in palavras) {
                    query = query.Where(r => r.Nome.ToLower().Contains(palavra));
                }

                return query;
            };

            return EfetuaBusca((set) => {
                var ordenado = set.OrderBy(r => r.Nome); 
                if (!string.IsNullOrWhiteSpace(filtro)) {
                    return geraFiltro(ordenado);
                }  else {
                    return ordenado;
                }
            });
        }

        public override void ValidarAlteracao(RestaurantePersistenciaModel novomodel)
        {
            if (string.IsNullOrWhiteSpace(novomodel.Nome))
            {
                throw new ValidacaoPersistenciaException(new[] {
                        new ErroValidacaoPropriedade("Nome", new[] { "nome obrigatório"})
                    });
            }
            else if (novomodel.Nome.Length > 100)
            {
                throw new ValidacaoPersistenciaException(new[] {
                        new ErroValidacaoPropriedade("Nome", new[] { "nome pode ter no máximo 100 caracteres"})
                    });
            }
        }

        public override void ValidarCriacao(RestaurantePersistenciaModel novomodel)
        {
            if (string.IsNullOrWhiteSpace(novomodel.Nome))
            {
                throw new ValidacaoPersistenciaException(new[] {
                        new ErroValidacaoPropriedade("Nome", new[] { "nome obrigatório"})
                    });
            }
            else if (novomodel.Nome.Length > 100)
            {
                throw new ValidacaoPersistenciaException(new[] {
                        new ErroValidacaoPropriedade("Nome", new[] { "nome pode ter no máximo 100 caracteres"})
                    });
            }
        }

        public override async Task Excluir(Guid id, bool persistir = true)
        {
            //Busca a entidade atual
            var restauranteExistente = await entidade_set
            .Include(e => e.Pratos)
            .FirstOrDefaultAsync(e => e.Id == id);

            if (restauranteExistente == null) {
                throw new EntidadeNaoExisteException();
            }

            foreach(var prato in restauranteExistente.Pratos) {
                prato.DataAlteracao = DateTimeOffset.Now;
                prato.Excluido = true;
            }
            
            restauranteExistente.DataAlteracao = DateTimeOffset.Now;
            restauranteExistente.Excluido = true;

            if (persistir)
            {
                await contexto.SaveChangesAsync();
            }
        }
    }
}