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
using System.Collections.Generic;

namespace test_api.servicos.persistencia
{

    public class ServicoPersistenciaBaseTests
    {
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
                var res = await servicoEmTeste.Criar(novomodel);
                res.Should().NotBeNull();
                res.Id.Should().NotBeEmpty();
                res.PropriedadeTeste.Should().Be("novo-valor");
            }

            using (var contextoVerificacao = new ContextoParaTeste(options))
            {
                //Verificar
                var entidades = await contextoVerificacao.Entidades.ToListAsync();
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
                var res = await servicoEmTeste.Alterar(modelalterado);

                res.Should().NotBeNull();
                res.Id.Should().Be(id);
                res.PropriedadeTeste.Should().Be("valor-alterado");
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
        public async Task BuscarPorId_OK()
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
                    DataAlteracao = DateTimeOffset.Now,
                    DataCriacao = DateTimeOffset.Now,
                    Excluido = false,
                    PropriedadeTeste = "valor-atual"
                };
                await contexto.AddAsync(entidadeExistente);
                await contexto.SaveChangesAsync();

                //Executar
                var resultado = await servicoEmTeste.BuscarPorId(id);

                //Verifica
                resultado.Should().BeEquivalentTo(new PersistenciaModelParaTeste
                {
                    Id = id,
                    PropriedadeTeste = "valor-atual"
                });
            }
        }

        [Fact]
        public void BuscarPorId_EntidadeNaoExiste_GeraException()
        {
            var options = OptionsContext();

            var id = Guid.NewGuid();
            using (var contexto = new ContextoParaTeste(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoEmTeste(contexto, BuscaMapper());

                //Executar
                Func<Task> act = async () => { await servicoEmTeste.BuscarPorId(id); };
                act.Should().Throw<EntidadeNaoExisteException>()
                    .WithMessage("Entidade não encontrada");
            }
        }

        [Fact]
        public async Task Buscar_OK()
        {
            var options = OptionsContext();

            var ids = new[] {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
            };

            using (var contexto = new ContextoParaTeste(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoEmTeste(contexto, BuscaMapper());

                var entidadesExistentes = new[] {
                    new EntidadeParaTeste {
                        Id = ids[0],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        PropriedadeTeste = "valor-atual-1"
                    },
                    new EntidadeParaTeste {
                        Id = ids[1],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        PropriedadeTeste = "valor-atual-2"
                    },
                    new EntidadeParaTeste {
                        Id = ids[2],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        PropriedadeTeste = "valor-atual-3"
                    },
                };
                await contexto.AddRangeAsync(entidadesExistentes);
                await contexto.SaveChangesAsync();

                //Executar
                var resultado = await servicoEmTeste.Buscar();

                //Verifica
                resultado.Should().BeEquivalentTo(new[] {
                    new PersistenciaModelParaTeste
                    {
                        Id = ids[0],
                        PropriedadeTeste = "valor-atual-1"
                    },
                    new PersistenciaModelParaTeste
                    {
                        Id = ids[1],
                        PropriedadeTeste = "valor-atual-2"
                    },
                    new PersistenciaModelParaTeste
                    {
                        Id = ids[2],
                        PropriedadeTeste = "valor-atual-3"
                    },
                });
            }
        }

        [Fact]
        public async Task Buscar_ComFiltro_OK()
        {
            //Retorna somente os filtrados
            var options = OptionsContext();

            var ids = new[] {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
            };

            using (var contexto = new ContextoParaTeste(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoEmTeste(contexto, BuscaMapper());

                var entidadesExistentes = new[] {
                    new EntidadeParaTeste {
                        Id = ids[0],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        PropriedadeTeste = "valor-atual-1"
                    },
                    new EntidadeParaTeste {
                        Id = ids[1],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        PropriedadeTeste = "valor-atual-2"
                    },
                    new EntidadeParaTeste {
                        Id = ids[2],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        PropriedadeTeste = "valor-atual-3"
                    },
                };
                await contexto.AddRangeAsync(entidadesExistentes);
                await contexto.SaveChangesAsync();

                //Executar
                var resultado = await servicoEmTeste.Buscar("valor-atual-2");

                //Verifica
                resultado.Should().BeEquivalentTo(new[] {
                    new PersistenciaModelParaTeste
                    {
                        Id = ids[1],
                        PropriedadeTeste = "valor-atual-2"
                    },
                });
            }
        }

        [Fact]
        public async Task Buscar_OrdenadosCorretamente_OK()
        {
            //Retorna ordenados
            var options = OptionsContext();

            var ids = new[] {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
            };

            using (var contexto = new ContextoParaTeste(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoEmTeste(contexto, BuscaMapper());

                var entidadesExistentes = new[] {
                    new EntidadeParaTeste {
                        Id = ids[1],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        PropriedadeTeste = "valor-atual-2"
                    },
                    new EntidadeParaTeste {
                        Id = ids[0],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        PropriedadeTeste = "valor-atual-1"
                    },
                    new EntidadeParaTeste {
                        Id = ids[2],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        PropriedadeTeste = "valor-atual-3"
                    },
                };
                await contexto.AddRangeAsync(entidadesExistentes);
                await contexto.SaveChangesAsync();

                //Executar
                var resultado = await servicoEmTeste.Buscar();

                //Verifica
                resultado.Should().BeEquivalentTo(new[] {
                    new PersistenciaModelParaTeste
                    {
                        Id = ids[0],
                        PropriedadeTeste = "valor-atual-1"
                    },
                    new PersistenciaModelParaTeste
                    {
                        Id = ids[1],
                        PropriedadeTeste = "valor-atual-2"
                    },
                    new PersistenciaModelParaTeste
                    {
                        Id = ids[2],
                        PropriedadeTeste = "valor-atual-3"
                    },
                }, opt => opt.WithStrictOrdering());
            }
        }

        //Auxiliares Teste
        private int intervaloDatas = 60000;
        private IMapper BuscaMapper()
        {
            MapperConfiguration automapper_configuration = new MapperConfiguration(cfg =>
            {
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

            public async Task<IEnumerable<PersistenciaModelParaTeste>> Buscar(string filtro = null)
            {
                return await EfetuaBusca((set) =>
                {
                    var ordenado = set.OrderBy(e => e.PropriedadeTeste);
                    if (string.IsNullOrWhiteSpace(filtro))
                    {
                        return ordenado;
                    }
                    else
                    {
                        return ordenado.Where(e => e.PropriedadeTeste == filtro);
                    }
                });
            }
        }

    }


}