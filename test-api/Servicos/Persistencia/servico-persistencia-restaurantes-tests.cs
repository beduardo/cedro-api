using AutoMapper;
using Microsoft.EntityFrameworkCore;
using api.data;
using api.servicos.persistencia;
using System;
using Xunit;
using System.Threading.Tasks;
using api.models;
using System.Linq;
using FluentAssertions;
using api.entidades;

namespace test_api.servicos.persistencia
{
    public class ServicoPersistenciaRestaurantesTests
    {
        [Fact]
        public void CriacaoServico_OK()
        {

            using (var contexto = new ContextoBdAplicacao(OptionsContext()))
            {
                var servicoEmTeste = new ServicoPersistenciaRestaurante(contexto, BuscaMapper());
            }
        }

        [Fact]
        public async Task CriarEntidade_OK()
        {
            var options = OptionsContext();

            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoPersistenciaRestaurante(contexto, BuscaMapper());

                //Executar
                var novomodel = new RestaurantePersistenciaModel
                {
                    Nome = "novo-restaurante"
                };
                await servicoEmTeste.Criar(novomodel);
            }

            using (var contextoVerificacao = new ContextoBdAplicacao(options))
            {
                //Verificar
                var entidades = await contextoVerificacao.Restaurantes.ToListAsync();
                entidades.Should().HaveCount(1);

                var entidade = entidades.FirstOrDefault();
                entidade.Id.Should().NotBeEmpty();
                entidade.DataAlteracao.Should().BeCloseTo(DateTimeOffset.Now, intervaloDatas);
                entidade.DataCriacao.Should().BeCloseTo(DateTimeOffset.Now, intervaloDatas);
                entidade.Excluido.Should().BeFalse();
                entidade.Nome.Should().Be("novo-restaurante");
            }
        }

        [Fact]
        public void CriarEntidadeInvalida_NomeVazio_GeraException()
        {
            var options = OptionsContext();

            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoPersistenciaRestaurante(contexto, BuscaMapper());

                //Executar
                var novomodel = new RestaurantePersistenciaModel();

                Func<Task> act = async () => { await servicoEmTeste.Criar(novomodel); };
                act.Should().Throw<ValidacaoPersistenciaException>()
                    .WithMessage("Entidade inválida")
                    .And.Erros.Should().BeEquivalentTo(new[] {
                        new ErroValidacaoPropriedade("Nome", new [] { "nome obrigatório" })
                    });
            }
        }

        [Fact]
        public void CriarEntidadeInvalida_NomeExcedeTamanho_GeraException()
        {
            var options = OptionsContext();

            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoPersistenciaRestaurante(contexto, BuscaMapper());

                //Executar
                var novomodel = new RestaurantePersistenciaModel
                {
                    Nome = new String('0', 101)
                };

                Func<Task> act = async () => { await servicoEmTeste.Criar(novomodel); };
                act.Should().Throw<ValidacaoPersistenciaException>()
                    .WithMessage("Entidade inválida")
                    .And.Erros.Should().BeEquivalentTo(new[] {
                        new ErroValidacaoPropriedade("Nome", new [] { "nome pode ter no máximo 100 caracteres" })
                    });
            }
        }

        [Fact]
        public async Task AlteraEntidade_OK()
        {
            var options = OptionsContext();
            var id = Guid.NewGuid();
            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                await contexto.AddAsync(new Restaurante
                {
                    Id = id,
                    DataCriacao = DateTimeOffset.Now,
                    Nome = "novo-restaurante"
                });
                await contexto.SaveChangesAsync();
                var servicoEmTeste = new ServicoPersistenciaRestaurante(contexto, BuscaMapper());

                //Executar
                var modelalterado = new RestaurantePersistenciaModel
                {
                    Id = id,
                    Nome = "restaurante-alterado"
                };
                await servicoEmTeste.Alterar(modelalterado);
            }

            using (var contextoVerificacao = new ContextoBdAplicacao(options))
            {
                //Verificar
                var entidades = await contextoVerificacao.Restaurantes.ToListAsync();
                entidades.Should().HaveCount(1);

                var entidade = entidades.FirstOrDefault();
                entidade.Id.Should().NotBeEmpty();
                entidade.DataAlteracao.Should().BeCloseTo(DateTimeOffset.Now, intervaloDatas);
                entidade.DataCriacao.Should().BeCloseTo(DateTimeOffset.Now, intervaloDatas);
                entidade.Excluido.Should().BeFalse();
                entidade.Nome.Should().Be("restaurante-alterado");
            }
        }

        [Fact]
        public async Task AlteraEntidadeInvalida_NomeVazio_GeraException()
        {
            var options = OptionsContext();
            var id = Guid.NewGuid();
            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                await contexto.AddAsync(new Restaurante
                {
                    Id = id,
                    DataCriacao = DateTimeOffset.Now,
                    Nome = "novo-restaurante"
                });
                await contexto.SaveChangesAsync();
                var servicoEmTeste = new ServicoPersistenciaRestaurante(contexto, BuscaMapper());

                //Executar
                var modeloalterado = new RestaurantePersistenciaModel
                {
                    Id = id
                };

                Func<Task> act = async () => { await servicoEmTeste.Alterar(modeloalterado); };
                act.Should().Throw<ValidacaoPersistenciaException>()
                    .WithMessage("Entidade inválida")
                    .And.Erros.Should().BeEquivalentTo(new[] {
                        new ErroValidacaoPropriedade("Nome", new [] { "nome obrigatório" })
                    });
            }
        }

        [Fact]
        public async Task AlteraEntidadeInvalida_NomeExcedeTamanho_GeraException()
        {
            var options = OptionsContext();
            var id = Guid.NewGuid();
            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                await contexto.AddAsync(new Restaurante
                {
                    Id = id,
                    DataCriacao = DateTimeOffset.Now,
                    Nome = "novo-restaurante"
                });
                await contexto.SaveChangesAsync();
                var servicoEmTeste = new ServicoPersistenciaRestaurante(contexto, BuscaMapper());

                //Executar
                var modeloalterado = new RestaurantePersistenciaModel
                {
                    Id = id,
                    Nome = new String('0', 101)
                };

                Func<Task> act = async () => { await servicoEmTeste.Alterar(modeloalterado); };
                act.Should().Throw<ValidacaoPersistenciaException>()
                    .WithMessage("Entidade inválida")
                    .And.Erros.Should().BeEquivalentTo(new[] {
                        new ErroValidacaoPropriedade("Nome", new [] { "nome pode ter no máximo 100 caracteres" })
                    });
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

            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoPersistenciaRestaurante(contexto, BuscaMapper());

                var entidadesExistentes = new[] {
                    new Restaurante {
                        Id = ids[0],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "restaurante azul"
                    },
                    new Restaurante {
                        Id = ids[1],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "restaurante azul e amarelo"
                    },
                    new Restaurante {
                        Id = ids[2],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "restaurante azul e vermelho"
                    },
                };
                await contexto.AddRangeAsync(entidadesExistentes);
                await contexto.SaveChangesAsync();

                //Executar
                var resultado = await servicoEmTeste.Buscar();

                //Verifica
                resultado.Should().BeEquivalentTo(new[] {
                    new RestaurantePersistenciaModel
                    {
                        Id = ids[0],
                        Nome = "restaurante azul"
                    },
                    new RestaurantePersistenciaModel
                    {
                        Id = ids[1],
                        Nome = "restaurante azul e amarelo"
                    },
                    new RestaurantePersistenciaModel
                    {
                        Id = ids[2],
                        Nome = "restaurante azul e vermelho"
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

            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoPersistenciaRestaurante(contexto, BuscaMapper());

                var entidadesExistentes = new[] {
                    new Restaurante {
                        Id = ids[0],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "Restaurante Azul"
                    },
                    new Restaurante {
                        Id = ids[1],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "Restaurante Azul e Amarelo"
                    },
                    new Restaurante {
                        Id = ids[2],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "Restaurante Azul e Vermelho"
                    },
                };
                await contexto.AddRangeAsync(entidadesExistentes);
                await contexto.SaveChangesAsync();

                //Executar
                var resultado = await servicoEmTeste.Buscar("azul amarelo");

                //Verifica
                resultado.Should().BeEquivalentTo(new[] {
                    new RestaurantePersistenciaModel
                    {
                        Id = ids[1],
                        Nome = "Restaurante Azul e Amarelo"
                    },
                });
            }
        }

        [Fact]
        public async Task Buscar_OrdenadosCorretamente_OK()
        {

            var options = OptionsContext();

            var ids = new[] {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
            };

            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoPersistenciaRestaurante(contexto, BuscaMapper());

                var entidadesExistentes = new[] {
                    new Restaurante {
                        Id = ids[0],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "restaurante azul"
                    },
                    new Restaurante {
                        Id = ids[2],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "restaurante azul e vermelho"
                    },
                    new Restaurante {
                        Id = ids[1],
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "restaurante azul e amarelo"
                    },
                };
                await contexto.AddRangeAsync(entidadesExistentes);
                await contexto.SaveChangesAsync();

                //Executar
                var resultado = await servicoEmTeste.Buscar();

                //Verifica
                resultado.Should().BeEquivalentTo(new[] {
                    new RestaurantePersistenciaModel
                    {
                        Id = ids[0],
                        Nome = "restaurante azul"
                    },
                    new RestaurantePersistenciaModel
                    {
                        Id = ids[1],
                        Nome = "restaurante azul e amarelo"
                    },
                    new RestaurantePersistenciaModel
                    {
                        Id = ids[2],
                        Nome = "restaurante azul e vermelho"
                    },
                }, opt => opt.WithStrictOrdering());
            }
        }

        //Auxiliares para teste
        private int intervaloDatas = 60000;
        private IMapper BuscaMapper()
        {
            MapperConfiguration automapper_configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<api.mapeamentos.ConfiguracaoAutoMapper>();

            });
            return automapper_configuration.CreateMapper();
        }

        private DbContextOptions<ContextoBdAplicacao> OptionsContext()
        {
            return new DbContextOptionsBuilder<ContextoBdAplicacao>()
                            .UseInMemoryDatabase(Guid.NewGuid().ToString())
                            .Options;
        }
    }
}