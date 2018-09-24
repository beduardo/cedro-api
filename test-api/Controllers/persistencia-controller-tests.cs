using Xunit;
using Moq;
using api.servicos.persistencia;
using api.models;
using api.controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace test_api.controllers
{
    public class PersistenciaControllerTests
    {

        [Fact]
        public void CriacaoController_OK()
        {
            //Prepara
            var servicoMock = new Mock<IServicoPersistenciaBase<EntidadeTestePersistenciaModel>>();
            var controllerEmTeste = new ControllerEmTeste(servicoMock.Object);
        }

        [Fact]
        public async Task BuscarPorId_OK()
        {
            //Prepara
            var idGerado = Guid.NewGuid();
            var response = new EntidadeTestePersistenciaModel
            {
                Id = idGerado,
                Idade = 10,
                Nome = "entidade-a",
            };

            var servicoMock = new Mock<IServicoPersistenciaBase<EntidadeTestePersistenciaModel>>();
            
            servicoMock
            .Setup(s => s.BuscarPorId(It.IsAny<Guid>()))
            .ReturnsAsync(response);

            var controllerEmTeste = new ControllerEmTeste(servicoMock.Object);
            var request = new EntidadeTestePersistenciaModel();

            //Executa
            var resp = await controllerEmTeste.buscarPorId(idGerado);

            //Verifica
            servicoMock.Verify(s => s.BuscarPorId(idGerado));
            resp.Should()
            .BeOfType<OkObjectResult>()
            .And
            .BeEquivalentTo(new OkObjectResult(response));
        }

        [Fact]
        public async Task BuscarPorId_NaoEncontrado_GeraException()
        {
            //Prepara
            var idGerado = Guid.NewGuid();
            var response = new EntidadeTestePersistenciaModel
            {
                Id = idGerado,
                Idade = 10,
                Nome = "entidade-a",
            };

            var servicoMock = new Mock<IServicoPersistenciaBase<EntidadeTestePersistenciaModel>>();
            
            servicoMock
            .Setup(s => s.BuscarPorId(It.IsAny<Guid>()))
            .ThrowsAsync(new EntidadeNaoExisteException());

            var controllerEmTeste = new ControllerEmTeste(servicoMock.Object);

            //Executa
            var resp = await controllerEmTeste.buscarPorId(idGerado);

            //Verifica
            resp.Should()
            .BeOfType<NotFoundObjectResult>();

            var notfound = resp as NotFoundObjectResult;
            notfound.Value.Should()
            .BeOfType<string>()
            .And
            .Be("Registro não encontrado");
        }


        [Fact]
        public async Task Criar_OK()
        {
            //Prepara
            var idGerado = Guid.NewGuid();
            var servicoMock = new Mock<IServicoPersistenciaBase<EntidadeTestePersistenciaModel>>();
            var retornoCriar = new EntidadeTestePersistenciaModel
            {
                Id = idGerado,
                Nome = "valor-retornado",
                Idade = 10
            };

            servicoMock
            .Setup(s => s.Criar(It.IsAny<EntidadeTestePersistenciaModel>(), It.IsAny<Boolean>()))
            .ReturnsAsync(retornoCriar);

            var controllerEmTeste = new ControllerEmTeste(servicoMock.Object);
            var request = new EntidadeTestePersistenciaModel();
            //Executa
            var resp = await controllerEmTeste.criar(request);

            //Verifica
            servicoMock.Verify(s => s.Criar(request, true));
            resp.Should()
            .BeOfType<CreatedAtActionResult>()
            .And
            .BeEquivalentTo(new CreatedAtActionResult("criar", null, null, retornoCriar));
        }

        [Fact]
        public async Task Criar_ExceptionValidacao_BadRequest()
        {
            //Prepara
            var servicoMock = new Mock<IServicoPersistenciaBase<EntidadeTestePersistenciaModel>>();

            servicoMock
            .Setup(s => s.Criar(It.IsAny<EntidadeTestePersistenciaModel>(), It.IsAny<Boolean>()))
            .ThrowsAsync(new ValidacaoPersistenciaException(new[] {
                new ErroValidacaoPropriedade("PROP1", new [] { "ERRO A" }),
                new ErroValidacaoPropriedade("PROP2", new [] { "ERRO B" }),
            }));

            var controllerEmTeste = new ControllerEmTeste(servicoMock.Object);
            var request = new EntidadeTestePersistenciaModel();
            //Executa
            var resp = await controllerEmTeste.criar(request);

            //Verifica
            resp.Should()
            .BeOfType<BadRequestObjectResult>();

            var badrequest = resp as BadRequestObjectResult;
            badrequest.Value.Should().BeOfType<ValidacaoPersistenciaException>();

            var validacaoexc = badrequest.Value as ValidacaoPersistenciaException;
            validacaoexc.Erros.Should().BeEquivalentTo(new[] {
                new ErroValidacaoPropriedade("PROP1", new [] { "ERRO A" }),
                new ErroValidacaoPropriedade("PROP2", new [] { "ERRO B" }),
            });
        }


        [Fact]
        public async Task Alterar_OK()
        {
            //Prepara
            var idGerado = Guid.NewGuid();
            var servicoMock = new Mock<IServicoPersistenciaBase<EntidadeTestePersistenciaModel>>();
            var retornoAlterar = new EntidadeTestePersistenciaModel
            {
                Id = idGerado,
                Nome = "valor-retornado",
                Idade = 10
            };

            servicoMock
            .Setup(s => s.Alterar(It.IsAny<EntidadeTestePersistenciaModel>(), It.IsAny<Boolean>()))
            .ReturnsAsync(retornoAlterar);

            var controllerEmTeste = new ControllerEmTeste(servicoMock.Object);
            var request = new EntidadeTestePersistenciaModel();
            //Executa
            var resp = await controllerEmTeste.alterar(request);

            //Verifica
            servicoMock.Verify(s => s.Alterar(request, true));
            resp.Should()
            .BeOfType<OkObjectResult>()
            .And
            .BeEquivalentTo(new OkObjectResult(retornoAlterar));
        }

        [Fact]
        public async Task Alterar_ExceptionValidacao_BadRequest()
        {
            //Prepara
            var servicoMock = new Mock<IServicoPersistenciaBase<EntidadeTestePersistenciaModel>>();

            servicoMock
            .Setup(s => s.Alterar(It.IsAny<EntidadeTestePersistenciaModel>(), It.IsAny<Boolean>()))
            .ThrowsAsync(new ValidacaoPersistenciaException(new[] {
                new ErroValidacaoPropriedade("PROP1", new [] { "ERRO A" }),
                new ErroValidacaoPropriedade("PROP2", new [] { "ERRO B" }),
            }));

            var controllerEmTeste = new ControllerEmTeste(servicoMock.Object);
            var request = new EntidadeTestePersistenciaModel();
            //Executa
            var resp = await controllerEmTeste.alterar(request);

            //Verifica
            resp.Should()
            .BeOfType<BadRequestObjectResult>();

            var badrequest = resp as BadRequestObjectResult;
            badrequest.Value.Should().BeOfType<ValidacaoPersistenciaException>();

            var validacaoexc = badrequest.Value as ValidacaoPersistenciaException;
            validacaoexc.Erros.Should().BeEquivalentTo(new[] {
                new ErroValidacaoPropriedade("PROP1", new [] { "ERRO A" }),
                new ErroValidacaoPropriedade("PROP2", new [] { "ERRO B" }),
            });
        }

        [Fact]
        public async Task Alterar_ExceptionNaoEncontrado_NotFound()
        {
            //Prepara
            var servicoMock = new Mock<IServicoPersistenciaBase<EntidadeTestePersistenciaModel>>();

            servicoMock
            .Setup(s => s.Alterar(It.IsAny<EntidadeTestePersistenciaModel>(), It.IsAny<Boolean>()))
            .ThrowsAsync(new EntidadeNaoExisteException());

            var controllerEmTeste = new ControllerEmTeste(servicoMock.Object);
            var request = new EntidadeTestePersistenciaModel();
            //Executa
            var resp = await controllerEmTeste.alterar(request);

            //Verifica
            resp.Should()
            .BeOfType<NotFoundObjectResult>();

            var notfound = resp as NotFoundObjectResult;
            notfound.Value.Should()
            .BeOfType<string>()
            .And
            .Be("Registro não encontrado");
        }

        [Fact]
        public async Task Excluir_OK()
        {
            //Prepara
            var idGerado = Guid.NewGuid();
            var servicoMock = new Mock<IServicoPersistenciaBase<EntidadeTestePersistenciaModel>>();

            var controllerEmTeste = new ControllerEmTeste(servicoMock.Object);
            var request = new EntidadeTestePersistenciaModel();

            //Executa
            var resp = await controllerEmTeste.excluir(idGerado);

            //Verifica
            servicoMock.Verify(s => s.Excluir(idGerado, true));

            resp.Should()
            .BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Excluir_ExceptionNaoEncontrado_NotFound()
        {
            //Prepara
            var idGerado = Guid.NewGuid();
            var servicoMock = new Mock<IServicoPersistenciaBase<EntidadeTestePersistenciaModel>>();

            servicoMock
            .Setup(s => s.Excluir(It.IsAny<Guid>(), It.IsAny<Boolean>()))
            .ThrowsAsync(new EntidadeNaoExisteException());

            var controllerEmTeste = new ControllerEmTeste(servicoMock.Object);
            //Executa
            var resp = await controllerEmTeste.excluir(idGerado);

            //Verifica
            resp.Should()
            .BeOfType<NotFoundObjectResult>();

            var notfound = resp as NotFoundObjectResult;
            notfound.Value.Should()
            .BeOfType<string>()
            .And
            .Be("Registro não encontrado");
        }

        public class ControllerEmTeste : PersistenciaController<EntidadeTestePersistenciaModel>
        {
            public ControllerEmTeste(IServicoPersistenciaBase<EntidadeTestePersistenciaModel> servico) : base(servico)
            {
            }
        }

        public class EntidadeTestePersistenciaModel : PersistenciaModelBase
        {
            public string Nome { get; set; }
            public int Idade { get; set; }
        }


    }
}