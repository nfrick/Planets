using Microsoft.EntityFrameworkCore;

namespace PlanetsAPI.Models {
    public class PlanetMContext : DbContext {
        public PlanetMContext(DbContextOptions<PlanetContext> options)
            : base(options) {
        }

        public DbSet<PlanetM> Planets { get; set; }
    }
}