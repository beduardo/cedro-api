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
    public class RestaurantesController : PersistenciaController<RestaurantePersistenciaModel>
    {
        private readonly IServicoPersistenciaRestaurante servico;
        private readonly IServicoPersistenciaPrato servicoPrato;
        public RestaurantesController(
            IServicoPersistenciaRestaurante servico,
            IServicoPersistenciaPrato servicoPrato
            ) : base(servico)
        {
            this.servico = servico;
            this.servicoPrato = servicoPrato;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> buscar(string filtro)
        {
            var res = await servico.Buscar(filtro);
            return Ok(res);
        }
    }
}