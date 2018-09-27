using System;
using System.Threading.Tasks;
using api.controllers;
using api.servicos.persistencia;
using api.models;
using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace test_api.controllers
{
    public class RestaurantesControllerTests
    {

        [Fact]
        public void CriacaoController_OK()
        {
            //Prepara
            var servicoMock = new Mock<IServicoPersistenciaRestaurante>();
            var servicoPratosMock = new Mock<IServicoPersistenciaPrato>();
            var controllerEmTeste = new RestaurantesController(servicoMock.Object, servicoPratosMock.Object);
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
                new RestaurantePersistenciaModel
                {
                    Id = ids[0],
                    Nome = "restaurante-a",
                },
                new RestaurantePersistenciaModel
                {
                    Id = ids[1],
                    Nome = "restaurante-b",
                },
                new RestaurantePersistenciaModel
                {
                    Id = ids[2],
                    Nome = "restaurante-c",
                },
                new RestaurantePersistenciaModel
                {
                    Id = ids[3],
                    Nome = "restaurante-d",
                }
            };

            var servicoMock = new Mock<IServicoPersistenciaRestaurante>();
            var servicoPratosMock = new Mock<IServicoPersistenciaPrato>();

            servicoMock
            .Setup(s => s.Buscar(It.IsAny<string>()))
            .ReturnsAsync(response);

            var controllerEmTeste = new RestaurantesController(servicoMock.Object, servicoPratosMock.Object);

            //Executa
            var resp = await controllerEmTeste.buscar(null);

            //Verifica
            servicoMock.Verify(s => s.Buscar(null));
            resp.Should()
            .BeOfType<OkObjectResult>()
            .And
            .BeEquivalentTo(new OkObjectResult(response));
        }

        [Fact]
        public async Task Buscar_ComFiltro_OK()
        {
            //Prepara
            var ids = new[] {
              Guid.NewGuid(),
              Guid.NewGuid(),
              Guid.NewGuid(),
              Guid.NewGuid()
            };

            var response = new[] {
                new RestaurantePersistenciaModel
                {
                    Id = ids[0],
                    Nome = "restaurante-a",
                },
                new RestaurantePersistenciaModel
                {
                    Id = ids[2],
                    Nome = "restaurante-c",
                },
            };

            var servicoMock = new Mock<IServicoPersistenciaRestaurante>();
            var servicoPratosMock = new Mock<IServicoPersistenciaPrato>();
            
            servicoMock
            .Setup(s => s.Buscar(It.IsAny<string>()))
            .ReturnsAsync(response);

            var controllerEmTeste = new RestaurantesController(servicoMock.Object, servicoPratosMock.Object);

            //Executa
            var resp = await controllerEmTeste.buscar("teste");

            //Verifica
            servicoMock.Verify(s => s.Buscar("teste"));
            resp.Should()
            .BeOfType<OkObjectResult>()
            .And
            .BeEquivalentTo(new OkObjectResult(response));
        }
    }
}