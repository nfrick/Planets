// Classe Planet para uso com banco de dados "in memory"

namespace PlanetsAPI.Models {
    public class Planet : PlanetBase {

        /// <summary>
        /// Id do planeta - gerado pelo banco de dados
        /// </summary>
        public long Id { get; set; }
    }
}