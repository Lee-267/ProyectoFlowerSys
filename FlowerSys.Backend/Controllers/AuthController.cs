using FlowerSys.Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.EntityFrameworkCore;
using FlowerSys.Backend.Entities;


namespace FlowerSys.Backend.Controllers
{
    [Route("api/[controller]")]
    
    [ApiController]
    public class AuthController(DataContext context, IConfiguration config) : ControllerBase
    {
        private readonly DataContext _context = context;
        private readonly string secretKey = config.GetSection("Jwt").GetValue<string>("key")!;

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == login.Email);

            if (usuario != null)
            {
                // Verifica si el usuario está activo
                if (!usuario.Activo)
                {
                    return Unauthorized(new { Message = "Usuario desactivado. No se puede iniciar sesión.", isSuccess = false, token = "" });
                }

                var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.Name, login.Email));
                claims.AddClaim(new Claim(ClaimTypes.Role, usuario.Perfil));
                claims.AddClaim(new Claim("Nombre", usuario.Nombre));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMonths(2),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                string tokencreado = tokenHandler.WriteToken(tokenConfig);

                if (BCrypt.Net.BCrypt.Verify(login.Password, usuario.Password))
                {
                    return Ok(new { Message = "Inicio de sesión exitoso.", isSuccess = true, token = tokencreado });
                }
            }

            return Unauthorized(new { Message = "Inicio de sesión fallido. Usuario o contraseña incorrectos.", isSuccess = false, token = "" });
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UsuarioDto model)
        {
            // Verifica si ya existe un usuario con el mismo correo
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                return BadRequest(new { Message = "El correo electrónico ya está registrado." });
            }

            // Crea un nuevo usuario
            var usuario = new Usuario
            {
                Nombre = model.Nombre,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Perfil = model.Perfil,
                FechaRegistro = DateTime.Now,
                Activo = true  // Asume que el usuario está activo por defecto
            };

            // Agrega el nuevo usuario a la base de datos
            _context.Users.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Usuario registrado exitosamente." });
        }
    }
}

