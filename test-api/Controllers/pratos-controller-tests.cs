using System;
using System.Threading.Tasks;
using api.controllers;
using api.models;
using api.servicos.persistencia;
using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace test_api.controllers
{
    public class PratosControllerTests
    {

        [Fact]
        public void CriacaoController_OK()
        {
            //Prepara
            var servicoMock = new Mock<IServicoPersistenciaPrato>();
            var controllerEmTeste = new PratosController(servicoMock.Object);
        }

        [Fact]
        public async Task Buscar_OK()
        {
            //Prepara
            var ids = new[] {
              Guid.NewGuid(),
              Guid.NewGuid(),
              Guid.NewGuid(),
              Guid.NewGuid()
            };

            var response = new[] {
                new PratoPersistenciaModel
                {
                    Id = ids[0],
                    Nome = "restaurante-a",
                },
                new PratoPersistenciaModel
                {
                    Id = ids[1],
                    Nome = "restaurante-b",
                },
                new PratoPersistenciaModel
                {
                    Id = ids[2],
                    Nome = "restaurante-c",
                },
                new PratoPersistenciaModel
                {
                    Id = ids[3],
                    Nome = "restaurante-d",
                }
            };

            var servicoMock = new Mock<IServicoPersistenciaPrato>();

            servicoMock
            .Setup(s => s.Buscar())
            .ReturnsAsync(response);

            var controllerEmTeste = new PratosController(servicoMock.Object);

            //Executa
            var resp = await controllerEmTeste.buscar();

            //Verifica
            servicoMock.Verify(s => s.Buscar());
            resp.Should()
            .BeOfType<OkObjectResult>()
            .And
            .BeEquivalentTo(new OkObjectResult(response));
        }

    }
}