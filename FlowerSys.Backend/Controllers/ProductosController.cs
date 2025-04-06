using FlowerSys.Backend.Entities;
using FlowerSys.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerSys.Backend.Controllers
{
   

    [Route("api/[controller]")]

    [ApiController]
   
    public class ProductosController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductosController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.Productos.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Productos producto)
        {
            // Verificar si ya existe un producto con el mismo nombre
            var productoExistente = await _context.Productos
                                                   .FirstOrDefaultAsync(p => p.Nombre == producto.Nombre);

            if (productoExistente != null)
            {
                // Si el producto ya existe, devolver un error
                return BadRequest("Ya existe un producto con ese nombre.");
            }

            // Si no existe, agregar el nuevo producto
            _context.Add(producto);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var producto = await _context.Productos
                .SingleOrDefaultAsync(p => p.Id == id);
            if (producto == null)
            {
                return NotFound();
            }
            return Ok(producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Productos producto)
        {
            if (id != producto.Id)
            {
                return BadRequest();
            }

            _context.Update(producto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            _context.Remove(producto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("vencimiento-proximo")]
        public async Task<IActionResult> GetExpiringSoonAsync()
        {
            DateTime fechaLimite = DateTime.Now.AddDays(7);
            var productos = await _context.Productos
                .Where(p => p.FechaVencimiento <= fechaLimite)
                .Select(p => new { p.Nombre, FechaVencimiento = p.FechaVencimiento.ToString("yyyy-MM-dd") })
                .ToListAsync();

            return Ok(new { mensaje = "Productos próximos a vencer obtenidos.", productos });
        }

        // Obtener productos con bajo stock (< 20)
        [HttpGet("bajo-stock")]
        public async Task<IActionResult> GetLowStockAsync()
        {
            var productos = await _context.Productos
                .Where(p => p.Stock < 20)
                .Select(p => new { p.Nombre, p.Stock })
                .ToListAsync();

            return Ok(new { mensaje = "Productos con bajo stock obtenidos.", productos });
        }


    }
}
