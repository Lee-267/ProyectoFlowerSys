using FlowerSys.Backend.Entities;
using FlowerSys.Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerSys.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialAccionController : ControllerBase
    {
        private DataContext _context;

        public HistorialAccionController(DataContext context)
        {
            {
                _context = context;
            }
        }

        [HttpGet]
        public async Task<IActionResult>GetAsync()
        {
            return Ok(await _context.HistorialAcciones.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(HistorialAccion historialAccion)
        {
            _context.Add(historialAccion);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("deleteall")]
        public async Task<IActionResult> DeleteAllHistorial()
        {
            try
            {
                // Verifica si hay elementos en la tabla HistorialAccion
                var historialAccion = await _context.HistorialAcciones.ToListAsync();

                if (historialAccion.Count == 0)
                {
                    return NotFound(new { message = "No hay historial para eliminar." });
                }
                _context.HistorialAcciones.RemoveRange(historialAccion);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Todo el historial ha sido eliminado." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el historial.", error = ex.Message });
            }
        }
    }
}
 