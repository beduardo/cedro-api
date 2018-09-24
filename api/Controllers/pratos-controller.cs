using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using api.models;
using api.servicos.persistencia;

namespace api.controllers
{
    [Route("api/pratos")]
    [ApiController]
    public class PratosController : PersistenciaController<PratoPersistenciaModel>
    {
        private readonly IServicoPersistenciaPrato servico;
        public PratosController(IServicoPersistenciaPrato servico) : base(servico) {
            this.servico = servico;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> buscar()
        {
            var res = await servico.Buscar();
            return Ok(res);
        }
    }
}