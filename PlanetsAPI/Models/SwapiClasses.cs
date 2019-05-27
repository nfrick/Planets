using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace PlanetsAPI.Models {
    // Classe para a leitura do json proveniente do swapi contendo vários planetas
    // Fornece planetas em "páginas" com 10 itens cada, e urls para páginas anterior e próxima
    internal class RootObject {
        [JsonProperty("count")]
        public int count { get; set; }  // Total de planetas no swapi
        [JsonProperty("next")]
        public string next { get; set; } // url para próxima "página" de planetas
        [JsonProperty("previous")]
        public string previous { get; set; } // url para "página" anterior de planetas
        [JsonProperty("results")]
        public List<PlanetSwapi> planets { get; set; } // Coleção de planetas
    }


    // Classe para a leitura do json proveniente do swapi contendo um planeta
    // A maior parte das propriedades não é utilizada
    // Propriedades aparentemente númericas foram declaradas string pois "unknown" é usado quando não disponível
    internal class PlanetSwapi {
        // Propriedades utilizadas
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("climate")]
        public string climate { get; set; }
        [JsonProperty("terrain")]
        public string terrain { get; set; }
        [JsonProperty("films")]
        public List<string> films { get; set; }  // Utilizado apenas o count
        [JsonProperty("url")]
        public string url { get; set; }  // Utilizado apenas o id

        // Propriedades swapi não utilizadas
        [JsonProperty("rotation_period")]
        public string rotation_period { get; set; }
        [JsonProperty("orbital_period")]
        public string orbital_period { get; set; }
        [JsonProperty("diameter")]
        public string diameter { get; set; }
        [JsonProperty("gravity")]
        public string gravity { get; set; }
        [JsonProperty("surface_water")]
        public string surface_water { get; set; }
        [JsonProperty("population")]
        public string population { get; set; }
        [JsonProperty("residents")]
        public List<string> residents { get; set; }
        [JsonProperty("created")]
        public DateTime created { get; set; }
        [JsonProperty("edited")]
        public DateTime edited { get; set; }
    }
}