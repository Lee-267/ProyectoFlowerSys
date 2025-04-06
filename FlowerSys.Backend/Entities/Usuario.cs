using System.ComponentModel.DataAnnotations;


namespace FlowerSys.Backend.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Perfil { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Activo { get; set; } = true; // Usuarios activos por defecto

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
      
    }
}
