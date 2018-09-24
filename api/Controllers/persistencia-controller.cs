using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using api.models;
using api.servicos.persistencia;

namespace api.controllers
{
    [ApiController]
    public abstract class PersistenciaController<TPersistenciaModel> : Controller
    where TPersistenciaModel : PersistenciaModelBase
    {
        private readonly IServicoPersistenciaBase<TPersistenciaModel> servico;
        public PersistenciaController(IServicoPersistenciaBase<TPersistenciaModel> servico)
        {
            this.servico = servico;
        }

        [Route("{id:guid}")]
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> buscarPorId(Guid id)
        {
            try
            {
                var res = await servico.BuscarPorId(id);
                return Ok(res);
            }
            catch (EntidadeNaoExisteException)
            {
                return NotFound("Registro não encontrado");
            }
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> criar([FromBody] TPersistenciaModel request)
        {
            try
            {
                var res = await servico.Criar(request, true);
                return CreatedAtAction(nameof(criar), res);
            }
            catch (ValidacaoPersistenciaException erro)
            {
                return BadRequest(erro);
            }
        }

        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> alterar([FromBody] TPersistenciaModel request)
        {
            try
            {
                var res = await servico.Alterar(request, true);
                return Ok(res);
            }
            catch (ValidacaoPersistenciaException erro)
            {
                return BadRequest(erro);
            }
            catch (EntidadeNaoExisteException)
            {
                return NotFound("Registro não encontrado");
            }
        }

        [Route("{id:guid}")]
        [HttpDelete]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> excluir(Guid Id)
        {
            try
            {
                await servico.Excluir(Id, true);
                return NoContent();
            }
            catch (EntidadeNaoExisteException)
            {
                return NotFound("Registro não encontrado");
            }
        }
    }
}