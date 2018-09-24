using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using api.models;
using api.servicos.persistencia;

namespace api.controllers
{
    [Route("api/restaurantes")]
    [ApiController]
    public class PratosController : Controller
    {
        private readonly IServicoPersistenciaPrato servico;
        public PratosController(IServicoPersistenciaPrato servico) {
            this.servico = servico;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult buscar(string filtro)
        {
            return Ok(new PratoPersistenciaModel[] {});
        }

        [Route("{id:guid}")]
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult buscarPorId(Guid id)
        {
            return Ok(new PratoPersistenciaModel());
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(201)]
        public IActionResult criar([FromBody] PratoPersistenciaModel request)
        {
            return CreatedAtAction(nameof(criar), new PratoPersistenciaModel());
        }

        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public IActionResult alterar([FromBody] PratoPersistenciaModel request)
        {
            return Ok(new PratoPersistenciaModel());
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