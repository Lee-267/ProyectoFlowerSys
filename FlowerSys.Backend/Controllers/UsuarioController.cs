using FlowerSys.Backend.Entities;
using FlowerSys.Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerSys.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController(DataContext context) : ControllerBase
    {
        private readonly DataContext _context = context;
        /*Lista de usuarios*/
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var usuarios = await _context.Users.ToListAsync();
            return Ok(usuarios);
        }
        /*usuarios por Id FILTRAR*/
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var usuario = await _context.Users.FindAsync(id);
            return Ok(usuario);
        }
        /*usuarios AGREGAR*/
      
        [HttpPost]
        public async Task<IActionResult> Post(Usuario usuario)
        {
            if (string.IsNullOrEmpty(usuario.Nombre) || string.IsNullOrEmpty(usuario.Email) || string.IsNullOrEmpty(usuario.Password))
            {
                return BadRequest(new { message = "Todos los campos son obligatorios." });
            }

            // Verifica si el correo ya está registrado
            var usuarioExistente = await _context.Users.FirstOrDefaultAsync(u => u.Email == usuario.Email);
            if (usuarioExistente != null)
            {
                return BadRequest(new { message = "El correo electrónico ya está registrado." });
            }

            // Encriptar contraseña antes de guardarla
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            usuario.Activo = true; // Asegurarse de que se registre activo por defecto

            try
            {
                _context.Users.Add(usuario);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Usuario registrado exitosamente.", isSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al registrar el usuario.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest(new { message = "ID de usuario no coincide." });
            }

            var usuarioExistente = await _context.Users.FindAsync(id);
            if (usuarioExistente == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            // Mantener la contraseña anterior si no se modificó
            if (string.IsNullOrEmpty(usuario.Password))
            {
                usuario.Password = usuarioExistente.Password;
            }
            else
            {
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            }

            // Mantener el estado de 'Activo'
            usuario.Activo = usuarioExistente.Activo;

            _context.Entry(usuarioExistente).CurrentValues.SetValues(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario actualizado correctamente." });
        }

    


        [HttpPatch("DesactivarUsuario/{id}")]
        public async Task<IActionResult> DesactivarUsuario(int id)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            if (!usuario.Activo) // Ya está desactivado
                return BadRequest(new { message = "El usuario ya está desactivado." });

            usuario.Activo = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario desactivado correctamente." });
        }

        [HttpPatch("ActivarUsuario/{id}")]
        public async Task<IActionResult> ActivarUsuario(int id)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            if (usuario.Activo) // Ya está activo
                return BadRequest(new { message = "El usuario ya está activo." });

            usuario.Activo = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario activado correctamente." });
        }

    }
}
