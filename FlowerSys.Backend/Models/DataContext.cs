using FlowerSys.Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowerSys.Backend.Models
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<Usuario> Users { get; set; }
        public DbSet<Productos> Productos { get; set; }

        public DbSet<Operacion> Operaciones { get; set; }

        public DbSet<HistorialAccion> HistorialAcciones { get; set;}
    }
}
