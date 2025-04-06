using System.ComponentModel.DataAnnotations;

namespace FlowerSys.Backend.Models
{
    public class UsuarioDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Perfil { get; set; } = string.Empty;



        [Display(Name = "Fecha de Creación")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
