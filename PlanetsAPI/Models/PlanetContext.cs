using Microsoft.EntityFrameworkCore;
using PlanetsAPI.Models;

namespace TodoApi.Models {
    public class PlanetContext : DbContext {
        public PlanetContext(DbContextOptions<PlanetContext> options)
            : base(options) {
        }

        public DbSet<Planet> Planets { get; set; }
    }
}
