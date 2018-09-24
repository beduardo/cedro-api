using System;
using System.Threading.Tasks;
using api.data;
using api.models;
using api.servicos.persistencia;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using api.entidades;
using AutoMapper;

namespace test_api.servicos.persistencia
{
    public class ServicoPersistenciaPratosTests
    {
        [Fact]
        public void CriacaoServico_OK()
        {

            using (var contexto = new ContextoBdAplicacao(OptionsContext()))
            {
                var servicoEmTeste = new ServicoPersistenciaPrato(contexto, BuscaMapper());
            }
        }

        [Fact]
        public async Task CriarEntidade_OK()
        {
            var options = OptionsContext();
            var idRestaurante = Guid.NewGuid();

            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                await contexto.AddAsync(new Restaurante
                {
                    Id = idRestaurante,
                    Nome = "restaurante"
                });
                await contexto.SaveChangesAsync();
                var servicoEmTeste = new ServicoPersistenciaPrato(contexto, BuscaMapper());

                //Executar
                var novomodel = new PratoPersistenciaModel
                {
                    RestauranteId = idRestaurante,
                    Nome = "novo-prato"
                };
                await servicoEmTeste.Criar(novomodel);
            }

            using (var contextoVerificacao = new ContextoBdAplicacao(options))
            {
                //Verificar
                var entidades = await contextoVerificacao.Pratos.ToListAsync();
                entidades.Should().HaveCount(1);

                var entidade = entidades.FirstOrDefault();
                entidade.Id.Should().NotBeEmpty();
                entidade.DataAlteracao.Should().BeCloseTo(DateTimeOffset.Now, intervaloDatas);
                entidade.DataCriacao.Should().BeCloseTo(DateTimeOffset.Now, intervaloDatas);
                entidade.Excluido.Should().BeFalse();
                entidade.Nome.Should().Be("novo-prato");
                entidade.RestauranteId.Should().Be(idRestaurante);
            }
        }

        [Fact]
        public async Task CriarEntidadeInvalida_RestauranteNaoInformado_GeraException()
        {
            var options = OptionsContext();
            var idRestaurante = Guid.NewGuid();

            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                await contexto.AddAsync(new Restaurante
                {
                    Id = idRestaurante,
                    Nome = "restaurante"
                });
                await contexto.SaveChangesAsync();
                var servicoEmTeste = new ServicoPersistenciaPrato(contexto, BuscaMapper());

                //Executar
                var novomodel = new PratoPersistenciaModel
                {
                    Nome = "novo-restaurante"
                };

                Func<Task> act = async () => { await servicoEmTeste.Criar(novomodel); };
                act.Should().Throw<ValidacaoPersistenciaException>()
                    .WithMessage("Entidade inválida")
                    .And.Erros.Should().BeEquivalentTo(new[] {
                        new ErroValidacaoPropriedade("Restaurante", new [] { "restaurante obrigatório" })
                    });
            }
        }

        [Fact]
        public async Task CriarEntidadeInvalida_NomeVazio_GeraException()
        {
            var options = OptionsContext();
            var idRestaurante = Guid.NewGuid();

            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                await contexto.AddAsync(new Restaurante
                {
                    Id = idRestaurante,
                    Nome = "restaurante"
                });
                await contexto.SaveChangesAsync();
                var servicoEmTeste = new ServicoPersistenciaPrato(contexto, BuscaMapper());

                //Executar
                var novomodel = new PratoPersistenciaModel
                {
                    RestauranteId = idRestaurante
                };

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
            var idRestaurante = Guid.NewGuid();

            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoPersistenciaPrato(contexto, BuscaMapper());

                //Executar
                var novomodel = new PratoPersistenciaModel
                {
                    RestauranteId = idRestaurante,
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
        public void CriarEntidadeInvalida_PrecoNegativo_GeraException()
        {
            var options = OptionsContext();
            var idRestaurante = Guid.NewGuid();

            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoPersistenciaPrato(contexto, BuscaMapper());

                //Executar
                var novomodel = new PratoPersistenciaModel
                {
                    RestauranteId = idRestaurante,
                    Nome = "prato-a",
                    Preco = -1
                };

                Func<Task> act = async () => { await servicoEmTeste.Criar(novomodel); };
                act.Should().Throw<ValidacaoPersistenciaException>()
                    .WithMessage("Entidade inválida")
                    .And.Erros.Should().BeEquivalentTo(new[] {
                        new ErroValidacaoPropriedade("Preco", new [] { "Preço não pode ser menor que zero" })
                    });
            }
        }

        [Fact]
        public async Task AlteraEntidade_OK()
        {
            var options = OptionsContext();
            var id = Guid.NewGuid();
            var idRestaurante = Guid.NewGuid();
            var idRestaurante2 = Guid.NewGuid();
            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                await contexto.AddAsync(new Restaurante
                {
                    Id = idRestaurante,
                    Nome = "restaurante-A"
                });
                await contexto.AddAsync(new Restaurante
                {
                    Id = idRestaurante2,
                    Nome = "restaurante-B"
                });
                await contexto.AddAsync(new Prato
                {
                    Id = id,
                    DataCriacao = DateTimeOffset.Now,
                    RestauranteId = idRestaurante,
                    Nome = "novo-prato"
                });
                await contexto.SaveChangesAsync();
                var servicoEmTeste = new ServicoPersistenciaPrato(contexto, BuscaMapper());

                //Executar
                var modelalterado = new PratoPersistenciaModel
                {
                    Id = id,
                    RestauranteId = idRestaurante2,
                    Nome = "prato-alterado"
                };
                await servicoEmTeste.Alterar(modelalterado);
            }

            using (var contextoVerificacao = new ContextoBdAplicacao(options))
            {
                //Verificar
                var entidades = await contextoVerificacao.Pratos.ToListAsync();
                entidades.Should().HaveCount(1);

                var entidade = entidades.FirstOrDefault();
                entidade.Id.Should().NotBeEmpty();
                entidade.DataAlteracao.Should().BeCloseTo(DateTimeOffset.Now, intervaloDatas);
                entidade.DataCriacao.Should().BeCloseTo(DateTimeOffset.Now, intervaloDatas);
                entidade.Excluido.Should().BeFalse();
                entidade.Nome.Should().Be("prato-alterado");
                entidade.RestauranteId.Should().Be(idRestaurante2);
            }
        }

        [Fact]
        public async Task AlteraEntidadeInvalida_NomeVazio_GeraException()
        {
            var options = OptionsContext();
            var id = Guid.NewGuid();
            var idRestaurante = Guid.NewGuid();
            var idRestaurante2 = Guid.NewGuid();
            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                await contexto.AddAsync(new Restaurante
                {
                    Id = idRestaurante,
                    Nome = "restaurante-A"
                });
                await contexto.AddAsync(new Restaurante
                {
                    Id = idRestaurante2,
                    Nome = "restaurante-B"
                });
                await contexto.AddAsync(new Prato
                {
                    Id = id,
                    DataCriacao = DateTimeOffset.Now,
                    RestauranteId = idRestaurante,
                    Nome = "novo-prato"
                });
                await contexto.SaveChangesAsync();
                var servicoEmTeste = new ServicoPersistenciaPrato(contexto, BuscaMapper());

                //Executar
                var modelalterado = new PratoPersistenciaModel
                {
                    Id = id,
                    RestauranteId = idRestaurante
                };

                Func<Task> act = async () => { await servicoEmTeste.Alterar(modelalterado); };
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
            var idRestaurante = Guid.NewGuid();
            var idRestaurante2 = Guid.NewGuid();
            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                await contexto.AddAsync(new Restaurante
                {
                    Id = idRestaurante,
                    Nome = "restaurante-A"
                });
                await contexto.AddAsync(new Restaurante
                {
                    Id = idRestaurante2,
                    Nome = "restaurante-B"
                });
                await contexto.AddAsync(new Prato
                {
                    Id = id,
                    DataCriacao = DateTimeOffset.Now,
                    RestauranteId = idRestaurante,
                    Nome = "novo-prato"
                });
                await contexto.SaveChangesAsync();
                var servicoEmTeste = new ServicoPersistenciaPrato(contexto, BuscaMapper());

                //Executar
                var modelalterado = new PratoPersistenciaModel
                {
                    Id = id,
                    RestauranteId = idRestaurante,
                    Nome = new String('0', 101)
                };

                Func<Task> act = async () => { await servicoEmTeste.Alterar(modelalterado); };
                act.Should().Throw<ValidacaoPersistenciaException>()
                    .WithMessage("Entidade inválida")
                    .And.Erros.Should().BeEquivalentTo(new[] {
                        new ErroValidacaoPropriedade("Nome", new [] { "nome pode ter no máximo 100 caracteres" })
                    });
            }
        }

        [Fact]
        public async Task AlteraEntidadeInvalida_PrecoMenorQueZero_GeraException()
        {
            var options = OptionsContext();
            var id = Guid.NewGuid();
            var idRestaurante = Guid.NewGuid();
            var idRestaurante2 = Guid.NewGuid();
            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                await contexto.AddAsync(new Restaurante
                {
                    Id = idRestaurante,
                    Nome = "restaurante-A"
                });
                await contexto.AddAsync(new Restaurante
                {
                    Id = idRestaurante2,
                    Nome = "restaurante-B"
                });
                await contexto.AddAsync(new Prato
                {
                    Id = id,
                    DataCriacao = DateTimeOffset.Now,
                    RestauranteId = idRestaurante,
                    Nome = "novo-prato"
                });
                await contexto.SaveChangesAsync();
                var servicoEmTeste = new ServicoPersistenciaPrato(contexto, BuscaMapper());

                //Executar
                var modelalterado = new PratoPersistenciaModel
                {
                    Id = id,
                    RestauranteId = idRestaurante,
                    Nome = "prato-alterado",
                    Preco = -1
                };

                Func<Task> act = async () => { await servicoEmTeste.Alterar(modelalterado); };
                act.Should().Throw<ValidacaoPersistenciaException>()
                    .WithMessage("Entidade inválida")
                    .And.Erros.Should().BeEquivalentTo(new[] {
                        new ErroValidacaoPropriedade("Preco", new [] { "Preço não pode ser menor que zero" })
                    });
            }
        }

        [Fact]
        public async Task Buscar_OK()
        {
            var options = OptionsContext();
            var idRestaurante = Guid.NewGuid();
            var idRestaurante2 = Guid.NewGuid();
            var ids = new[] {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
            };

            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoPersistenciaPrato(contexto, BuscaMapper());

                await contexto.AddAsync(new Restaurante
                {
                    Id = idRestaurante,
                    Nome = "restaurante-a"
                });

                await contexto.AddAsync(new Restaurante
                {
                    Id = idRestaurante2,
                    Nome = "restaurante-b"
                });

                var entidadesExistentes = new[] {
                    new Prato {
                        Id = ids[0],
                        RestauranteId = idRestaurante,
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "prato-a"
                    },
                    new Prato {
                        Id = ids[1],
                        RestauranteId = idRestaurante2,
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "prato-b"
                    },
                    new Prato {
                        Id = ids[2],
                        RestauranteId = idRestaurante,
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "prato-c"
                    },
                };
                await contexto.AddRangeAsync(entidadesExistentes);
                await contexto.SaveChangesAsync();

                //Executar
                var resultado = await servicoEmTeste.Buscar();

                //Verifica
                resultado.Should().BeEquivalentTo(new[] {
                    new PratoPersistenciaModel
                    {
                        Id = ids[0],
                        RestauranteId = idRestaurante,
                        RestauranteNome = "restaurante-a",
                        Nome = "prato-a"
                    },
                    new PratoPersistenciaModel
                    {
                        Id = ids[1],
                        RestauranteId = idRestaurante2,
                        RestauranteNome = "restaurante-b",
                        Nome = "prato-b"
                    },
                    new PratoPersistenciaModel
                    {
                        Id = ids[2],
                        RestauranteId = idRestaurante,
                        RestauranteNome = "restaurante-a",
                        Nome = "prato-c"
                    },
                });
            }
        }

        [Fact]
        public async Task Buscar_Ordenado_OK()
        {
            var options = OptionsContext();
            var idRestaurante = Guid.NewGuid();
            var idRestaurante2 = Guid.NewGuid();
            var ids = new[] {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
            };

            using (var contexto = new ContextoBdAplicacao(options))
            {
                //Preparar
                var servicoEmTeste = new ServicoPersistenciaPrato(contexto, BuscaMapper());

                await contexto.AddAsync(new Restaurante
                {
                    Id = idRestaurante,
                    Nome = "restaurante-a"
                });

                await contexto.AddAsync(new Restaurante
                {
                    Id = idRestaurante2,
                    Nome = "restaurante-b"
                });

                var entidadesExistentes = new[] {
                    new Prato {
                        Id = ids[2],
                        RestauranteId = idRestaurante,
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "prato-c"
                    },
                    new Prato {
                        Id = ids[0],
                        RestauranteId = idRestaurante,
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "prato-a"
                    },
                    new Prato {
                        Id = ids[1],
                        RestauranteId = idRestaurante2,
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "prato-b"
                    },
                    new Prato {
                        Id = ids[4],
                        RestauranteId = idRestaurante2,
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "prato-e"
                    },
                    new Prato {
                        Id = ids[3],
                        RestauranteId = idRestaurante2,
                        DataAlteracao = DateTimeOffset.Now,
                        DataCriacao = DateTimeOffset.Now,
                        Excluido = false,
                        Nome = "prato-d"
                    },
                };
                await contexto.AddRangeAsync(entidadesExistentes);
                await contexto.SaveChangesAsync();

                //Executar
                var resultado = await servicoEmTeste.Buscar();

                //Verifica
                resultado.Should().BeEquivalentTo(new[] {
                    new PratoPersistenciaModel
                    {
                        Id = ids[0],
                        RestauranteId = idRestaurante,
                        RestauranteNome = "restaurante-a",
                        Nome = "prato-a"
                    },
                    new PratoPersistenciaModel
                    {
                        Id = ids[2],
                        RestauranteId = idRestaurante,
                        RestauranteNome = "restaurante-a",
                        Nome = "prato-c"
                    },
                    new PratoPersistenciaModel
                    {
                        Id = ids[1],
                        RestauranteId = idRestaurante2,
                        RestauranteNome = "restaurante-b",
                        Nome = "prato-b"
                    },
                    new PratoPersistenciaModel
                    {
                        Id = ids[3],
                        RestauranteId = idRestaurante2,
                        RestauranteNome = "restaurante-b",
                        Nome = "prato-d"
                    },
                    new PratoPersistenciaModel
                    {
                        Id = ids[4],
                        RestauranteId = idRestaurante2,
                        RestauranteNome = "restaurante-b",
                        Nome = "prato-e"
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