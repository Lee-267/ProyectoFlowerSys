namespace FlowerSys.Backend.Entities
{
    public class HistorialAccion
    {
        public int Id { get; set; }
        public string Accion { get; set; } = null!;

        public DateTime FechaAccion { get; set; }
    }
}
