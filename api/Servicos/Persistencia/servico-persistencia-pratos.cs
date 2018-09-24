using api.entidades;
using api.models;
using api.data;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace api.servicos.persistencia
{
    public interface IServicoPersistenciaPrato : IServicoPersistenciaBase<PratoPersistenciaModel>
    {

        Task<IEnumerable<PratoPersistenciaModel>> Buscar();
    }

    public class ServicoPersistenciaPrato : ServicoPersistenciaBase<Prato, PratoPersistenciaModel>, IServicoPersistenciaPrato
    {
        public ServicoPersistenciaPrato(ContextoBdAplicacao contexto, IMapper mapeador) : base(contexto, mapeador)
        {

        }

        public Task<IEnumerable<PratoPersistenciaModel>> Buscar()
        {
            return EfetuaBusca(set => set
            .OrderBy(p => p.Restaurante.Nome)
            .ThenBy(p => p.Nome)
            .Include(p => p.Restaurante));
        }

        public override void ValidarAlteracao(PratoPersistenciaModel novomodel)
        {
            var erros = new List<ErroValidacaoPropriedade>();

            if (novomodel.RestauranteId == System.Guid.Empty)
            {
                erros.Add(new ErroValidacaoPropriedade("Restaurante", new[] { "restaurante obrigatório" }));
            }
            
            if (string.IsNullOrWhiteSpace(novomodel.Nome))
            {
                erros.Add(new ErroValidacaoPropriedade("Nome", new[] { "nome obrigatório"}));
            } else if (novomodel.Nome.Length > 100)
            {
                erros.Add(new ErroValidacaoPropriedade("Nome", new[] { "nome pode ter no máximo 100 caracteres"}));
            }
            
            if (novomodel.Preco < 0)
            {
                erros.Add(new ErroValidacaoPropriedade("Preco", new[] { "Preço não pode ser menor que zero"}));
            }

            if (erros.Count > 0)
            {
                throw new ValidacaoPersistenciaException(erros.ToArray());
            }
        }

        public override void ValidarCriacao(PratoPersistenciaModel novomodel)
        {
            var erros = new List<ErroValidacaoPropriedade>();

            if (novomodel.RestauranteId == System.Guid.Empty)
            {
                erros.Add(new ErroValidacaoPropriedade("Restaurante", new[] { "restaurante obrigatório" }));
            }
            
            if (string.IsNullOrWhiteSpace(novomodel.Nome))
            {
                erros.Add(new ErroValidacaoPropriedade("Nome", new[] { "nome obrigatório"}));
            } else if (novomodel.Nome.Length > 100)
            {
                erros.Add(new ErroValidacaoPropriedade("Nome", new[] { "nome pode ter no máximo 100 caracteres"}));
            }
            
            if (novomodel.Preco < 0)
            {
                erros.Add(new ErroValidacaoPropriedade("Preco", new[] { "Preço não pode ser menor que zero"}));
            }

            if (erros.Count > 0)
            {
                throw new ValidacaoPersistenciaException(erros.ToArray());
            }
        }
    }
}