using System.Threading.Tasks;
using api.data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace api.controllers
{
    [Route("api/restaurantes")]
    [ApiController]
    public class RestaurantesController : Controller
    {
        private readonly ContextoBdAplicacao contexto;
        public RestaurantesController(ContextoBdAplicacao contexto)
        {
            this.contexto = contexto;
        }

        [HttpGet]
        public async Task<IActionResult> buscartodos()
        {
            var resultado = await contexto.Restaurantes.ToListAsync();
            return Ok(resultado);
        }
    }
}