using System;
using System.Linq;
using api.entidades;
using api.servicos.persistencia;
using api.data;
using Xunit;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using System.Threading.Tasks;
using AutoMapper;
using api.mapeamentos;
using api.models;

namespace test_api.servicos.persistencia
{

    public class ServicoPersistenciaBaseTests
    {
        private int intervaloDatas = 60000;
        private IMapper BuscaMapper()
        {
            MapperConfiguration automapper_configuration = new MapperConfiguration(cfg =>
            {
                // cfg.AddProfile<ConfiguracaoAutoMapper>();
                cfg.CreateMap<EntidadeParaTeste, PersistenciaModelParaTeste>();
                cfg.CreateMap<PersistenciaModelParaTeste, EntidadeParaTeste>()
                    .IgnorePadraoEntidade();

            });
            return automapper_configuration.CreateMapper();
        }

        private DbContextOptions<ContextoParaTeste> OptionsContext()
        {
            return new DbContextOptionsBuilder<ContextoParaTeste>()
                            .UseInMemoryDatabase(Guid.NewGuid().ToString())
                            .Options;
        }

        [Fact]
        public void CriacaoServico_OK()
        {
            using (var contexto = new ContextoParaTeste(OptionsContext()))
            {
                var servicoEmTeste = new ServicoEmTeste(contexto, BuscaMapper());
            }
        }

        [Fact]
        public async Task CriarEntidade_OK()
        {
            var options = OptionsContext();

            using (var contexto = new ContextoParaTeste(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoEmTeste(contexto, BuscaMapper());

                //Executar
                var novomodel = new PersistenciaModelParaTeste
                {
                    PropriedadeTeste = "novo-valor"
                };
                await servicoEmTeste.Criar(novomodel);
            }

            using (var contextoVerificacao = new ContextoParaTeste(options))
            {
                //Verificar
                var entidades = contextoVerificacao.Entidades.ToList();
                entidades.Should().HaveCount(1);

                var entidade = entidades.FirstOrDefault();
                entidade.Id.Should().NotBeEmpty();
                entidade.DataAlteracao.Should().BeCloseTo(DateTimeOffset.Now, intervaloDatas);
                entidade.DataCriacao.Should().BeCloseTo(DateTimeOffset.Now, intervaloDatas);
                entidade.Excluido.Should().BeFalse();
                entidade.PropriedadeTeste.Should().Be("novo-valor");
            }
        }

        [Fact]
        public void CriarEntidadeInvalida_GeraException()
        {
            var options = OptionsContext();

            using (var contexto = new ContextoParaTeste(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoEmTeste(contexto, BuscaMapper());

                //Executar
                var novomodel = new PersistenciaModelParaTeste();

                Func<Task> act = async () => { await servicoEmTeste.Criar(novomodel); };
                act.Should().Throw<ValidacaoPersistenciaException>()
                    .WithMessage("Entidade inválida")
                    .And.Erros.Should().BeEquivalentTo(new[] {
                        new ErroValidacaoPropriedade("PropriedadeTeste", new [] { "propriedade obrigatória" })
                    });
            }
        }

        [Fact]
        public async Task CriarSemPersistirEntidade_OK()
        {
            var options = OptionsContext();

            using (var contexto = new ContextoParaTeste(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoEmTeste(contexto, BuscaMapper());

                //Executar
                var novomodel = new PersistenciaModelParaTeste
                {
                    PropriedadeTeste = "novo-valor"
                };
                await servicoEmTeste.Criar(novomodel, false);
            }

            using (var contextoVerificacao = new ContextoParaTeste(options))
            {

                //Verificar
                var entidades = contextoVerificacao.Entidades.ToList();
                entidades.Should().BeEmpty();

            }
        }

        [Fact]
        public async Task AlterarEntidade_OK()
        {
            var options = OptionsContext();

            var id = Guid.NewGuid();
            using (var contexto = new ContextoParaTeste(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoEmTeste(contexto, BuscaMapper());

                var entidadeExistente = new EntidadeParaTeste
                {
                    Id = id,
                    DataCriacao = DateTimeOffset.Now,
                    PropriedadeTeste = "valor-existente"
                };
                await contexto.AddAsync(entidadeExistente);
                await contexto.SaveChangesAsync();

                //Executar
                var modelalterado = new PersistenciaModelParaTeste
                {
                    Id = id,
                    PropriedadeTeste = "valor-alterado"
                };
                await servicoEmTeste.Alterar(modelalterado);
            }

            using (var contextoVerificacao = new ContextoParaTeste(options))
            {

                //Verificar
                var entidades = contextoVerificacao.Entidades.ToList();
                entidades.Should().HaveCount(1);

                var entidade = entidades.FirstOrDefault();
                entidade.Id.Should().Be(id);
                entidade.DataAlteracao.Should().BeCloseTo(DateTimeOffset.Now, intervaloDatas);
                entidade.DataCriacao.Should().BeCloseTo(DateTimeOffset.Now, intervaloDatas);
                entidade.Excluido.Should().BeFalse();
                entidade.PropriedadeTeste.Should().Be("valor-alterado");
            }
        }

        [Fact]
        public async Task AlterarEntidadeInvalida_GeraException()
        {
            var options = OptionsContext();

            var id = Guid.NewGuid();
            using (var contexto = new ContextoParaTeste(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoEmTeste(contexto, BuscaMapper());

                var entidadeExistente = new EntidadeParaTeste
                {
                    Id = id,
                    DataCriacao = DateTimeOffset.Now,
                    PropriedadeTeste = "valor-existente"
                };
                await contexto.AddAsync(entidadeExistente);
                await contexto.SaveChangesAsync();

                //Executar
                var modelalterado = new PersistenciaModelParaTeste
                {
                    Id = id
                };

                Func<Task> act = async () => { await servicoEmTeste.Alterar(modelalterado); };
                act.Should().Throw<ValidacaoPersistenciaException>()
                    .WithMessage("Entidade inválida")
                    .And.Erros.Should().BeEquivalentTo(new[] {
                        new ErroValidacaoPropriedade("PropriedadeTeste", new [] { "propriedade obrigatória" })
                    });
            }
        }

        [Fact]
        public void AlterarEntidadeNaoExistente_GeraException()
        {
            var options = OptionsContext();

            var id = Guid.NewGuid();
            using (var contexto = new ContextoParaTeste(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoEmTeste(contexto, BuscaMapper());

                //Executar
                var modelalterado = new PersistenciaModelParaTeste
                {
                    Id = id,
                    PropriedadeTeste = "valor-alterado"
                };

                Func<Task> act = async () => { await servicoEmTeste.Alterar(modelalterado); };
                act.Should().Throw<EntidadeNaoExisteException>()
                    .WithMessage("Entidade não encontrada");
            }
        }

        [Fact]
        public async Task AlterarSemPersistirEntidade_OK()
        {
            var options = OptionsContext();

            var id = Guid.NewGuid();
            using (var contexto = new ContextoParaTeste(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoEmTeste(contexto, BuscaMapper());

                var entidadeExistente = new EntidadeParaTeste
                {
                    Id = id,
                    DataCriacao = DateTimeOffset.Now,
                    PropriedadeTeste = "valor-existente"
                };
                await contexto.AddAsync(entidadeExistente);
                await contexto.SaveChangesAsync();

                //Executar
                var modoalterado = new PersistenciaModelParaTeste
                {
                    Id = id,
                    PropriedadeTeste = "valor-alterado"
                };
                await servicoEmTeste.Alterar(modoalterado, false);
            }

            using (var contextoVerificacao = new ContextoParaTeste(options))
            {

                //Verificar
                var entidades = contextoVerificacao.Entidades.ToList();
                entidades.Should().HaveCount(1);

                var entidade = entidades.FirstOrDefault();
                entidade.Id.Should().Be(id);
                entidade.DataCriacao.Should().BeCloseTo(DateTimeOffset.Now, intervaloDatas);
                entidade.Excluido.Should().BeFalse();
                entidade.PropriedadeTeste.Should().Be("valor-existente");
            }
        }

        [Fact]
        public async Task ExcluirEntidade_OK()
        {
            var options = OptionsContext();

            var id = Guid.NewGuid();
            using (var contexto = new ContextoParaTeste(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoEmTeste(contexto, BuscaMapper());

                var entidadeExistente = new EntidadeParaTeste
                {
                    Id = id,
                    DataCriacao = DateTimeOffset.Now,
                    PropriedadeTeste = "valor-atual"
                };
                await contexto.AddAsync(entidadeExistente);
                await contexto.SaveChangesAsync();

                //Executar
                await servicoEmTeste.Excluir(id);
            }

            using (var contextoVerificacao = new ContextoParaTeste(options))
            {

                //Verificar
                var entidades = contextoVerificacao.Entidades.ToList();
                entidades.Should().HaveCount(1);

                var entidade = entidades.FirstOrDefault();
                entidade.Id.Should().Be(id);
                entidade.DataAlteracao.Should().BeCloseTo(DateTimeOffset.Now, intervaloDatas);
                entidade.DataCriacao.Should().BeCloseTo(DateTimeOffset.Now, intervaloDatas);
                entidade.Excluido.Should().BeTrue();
                entidade.PropriedadeTeste.Should().Be("valor-atual");
            }
        }

        [Fact]
        public void ExcluirEntidadeNaoExistente_GeraException()
        {
            var options = OptionsContext();

            var id = Guid.NewGuid();
            using (var contexto = new ContextoParaTeste(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoEmTeste(contexto, BuscaMapper());

                //Executar
                Func<Task> act = async () => { await servicoEmTeste.Excluir(id); };
                act.Should().Throw<EntidadeNaoExisteException>()
                    .WithMessage("Entidade não encontrada");
            }
        }

        [Fact]
        public async Task ExcluirSemPersistirEntidade_OK()
        {
            var options = OptionsContext();

            var id = Guid.NewGuid();
            using (var contexto = new ContextoParaTeste(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoEmTeste(contexto, BuscaMapper());

                var entidadeExistente = new EntidadeParaTeste
                {
                    Id = id,
                    DataCriacao = DateTimeOffset.Now,
                    PropriedadeTeste = "valor-atual"
                };
                await contexto.AddAsync(entidadeExistente);
                await contexto.SaveChangesAsync();

                //Executar
                await servicoEmTeste.Excluir(id, false);
            }

            using (var contextoVerificacao = new ContextoParaTeste(options))
            {

                //Verificar
                var entidades = contextoVerificacao.Entidades.ToList();
                entidades.Should().HaveCount(1);

                var entidade = entidades.FirstOrDefault();
                entidade.Id.Should().Be(id);
                entidade.DataCriacao.Should().BeCloseTo(DateTimeOffset.Now, intervaloDatas);
                entidade.Excluido.Should().BeFalse();
                entidade.PropriedadeTeste.Should().Be("valor-atual");
            }
        }

        [Fact]
        public void BuscarPorId_OK() {
            throw new NotImplementedException();
        }

        [Fact]
        public void BuscarPorId_EntidadeNaoExiste_GeraException() {
            throw new NotImplementedException();
        }

        [Fact]
        public void Buscar_OK() {
            //Retorna todos
            throw new NotImplementedException();
        }

        [Fact]
        public void Buscar_ComFiltro_OK() {
            //Retorna somente os filtrados
            throw new NotImplementedException();
        }

        [Fact]
        public void Buscar_OrdenadosCorretamente_OK() {
            //Retorna somente os filtrados
            throw new NotImplementedException();
        }

        //Auxiliares Teste
        private class EntidadeParaTeste : EntidadeBase
        {
            public string PropriedadeTeste { get; set; }
        }

        private class PersistenciaModelParaTeste : PersistenciaModelBase
        {
            public string PropriedadeTeste { get; set; }
        }

        private class ContextoParaTeste : DbContext
        {
            public ContextoParaTeste(DbContextOptions<ContextoParaTeste> options) : base(options) { }

            public DbSet<EntidadeParaTeste> Entidades { get; set; }
        }

        private class ServicoEmTeste : ServicoPersistenciaBase<EntidadeParaTeste, PersistenciaModelParaTeste>
        {
            public ServicoEmTeste(ContextoParaTeste ctx, IMapper mapeador) : base(ctx, mapeador)
            {
            }

            public override void ValidarAlteracao(PersistenciaModelParaTeste novomodel)
            {
                if (string.IsNullOrWhiteSpace(novomodel.PropriedadeTeste))
                {
                    throw new ValidacaoPersistenciaException(new[] {
                        new ErroValidacaoPropriedade("PropriedadeTeste", new[] { "propriedade obrigatória"})
                    });
                }
            }

            public override void ValidarCriacao(PersistenciaModelParaTeste novomodel)
            {
                if (string.IsNullOrWhiteSpace(novomodel.PropriedadeTeste))
                {
                    throw new ValidacaoPersistenciaException(new[] {
                        new ErroValidacaoPropriedade("PropriedadeTeste", new[] { "propriedade obrigatória"})
                    });
                }
            }
        }

    }


}