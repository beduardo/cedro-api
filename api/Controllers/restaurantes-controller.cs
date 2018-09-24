using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using api.models;
using api.servicos.persistencia;

namespace api.controllers
{
    [Route("api/restaurantes")]
    [ApiController]
    public class RestaurantesController : Controller
    {
        private readonly IServicoPersistenciaRestaurante servico;
        public RestaurantesController(IServicoPersistenciaRestaurante servico) {
            this.servico = servico;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult buscar(string filtro)
        {
            return Ok(new RestaurantePersistenciaModel[] {});
        }

        [Route("{id:guid}")]
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult buscarPorId(Guid id)
        {
            return Ok(new RestaurantePersistenciaModel());
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(201)]
        public IActionResult criar([FromBody] RestaurantePersistenciaModel request)
        {
            return CreatedAtAction(nameof(criar), new RestaurantePersistenciaModel());
        }

        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public IActionResult alterar([FromBody] RestaurantePersistenciaModel request)
        {
            return Ok(new RestaurantePersistenciaModel());
        }

        [Route("{id:guid}")]
        [HttpDelete]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public IActionResult excluir(Guid Id)
        {
            return Ok();
        }
    }
}