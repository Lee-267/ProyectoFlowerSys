using FlowerSys.Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowerSys.Backend.Models
{
    public class SeedDb(DataContext context)
    {
        // Constructor
        private readonly DataContext _context = context;

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckUsersAsync("admin", "admin@gmail.com", "adm123", "Administrador");
        }

        private async Task<Usuario> CheckUsersAsync(string nombre, string correo, string password, string perfil)
        {/// Busca un usuario con el correo proporcionado
            var usuarioExistente = await _context.Users.FirstOrDefaultAsync(u => u.Email == correo);
            if (usuarioExistente != null)
            {
                return usuarioExistente!;
            }
            // Si no existe, se crea uno nuevo
            Usuario usuario = new()
            {
                Email = correo,
                Nombre = nombre,
                Perfil = perfil,
                Password = password,
                Activo = true // El usuario está activo por defecto
            };
            // Encriptar la contraseña
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            // Agregar el nuevo usuario a la base de datos
            _context.Users.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }
    }
}
