using Microsoft.EntityFrameworkCore;

namespace PlanetsAPI.Models {
    public class PlanetContext : DbContext {
        public PlanetContext(DbContextOptions<PlanetContext> options)
            : base(options) {
        }

        public DbSet<Planet> Planets { get; set; }
    }
}
