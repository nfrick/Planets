using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PlanetsAPI.Models {
    public class Planet {
        private const string swapi = @"https://swapi.co/api/planets";
        public long Id { get; set; }  // Gerado pelo banco de dados
        public string Name { get; set; }  // Fornecido pelo usuário ou obtido no swapi
        public string Climate { get; set; }  // Fornecido pelo usuário ou obtido no swapi
        public string Terrain { get; set; }  // Fornecido pelo usuário ou obtido no swapi
        public int Movies { get; private set; }  // Obtido no swapi se for planeta Star Wars, senão 0
        public int SwapiId { get; private set; }  // Obtido no swapi se for planeta Star Wars, senão 0
        public string SwapiURL => IsStarWars ? $"{swapi}/{SwapiId}/" : null;  // Para facilitar as buscas, não é salvo no BD
        public bool IsStarWars => SwapiId != 0;

        /// <summary>
        /// Sincroniza o planeta com os dados obtidos no sawpi, se disponíveis.
        /// É chamado na criação de novo planeta ou atualização de dados.
        /// </summary>
        public void SyncWithSwapi() {
            using (var web = new WebClient()) {
                try {
                    if (IsStarWars) {  // Planeta já existe no nosso BD e tem correspondente no swapi
                        if (GetSwapiPlanet(web, SwapiURL)) {
                            return;
                        }
                    }
                    else if (int.TryParse(Name, out int id)) {  // Nome do planeta é uma id do swapi
                        if (GetSwapiPlanet(web, $"{swapi}/{Name}/")) {
                            return;
                        }
                    }

                    // Procurar planeta no swapi por nome.
                    // O swapi fornece planetas em páginas com 10 planetas cada.
                    // Cada página tem o link para a página seguinte.
                    var request = swapi;  // Página inicial de planetas
                    do {
                        var json = web.DownloadString(request);
                        var result = JsonConvert.DeserializeObject<RootObject>(json);
                        // No "lote" de 10 planetas, procuramos o nosso alvo, "case insensitive"
                        var found = result.planets.FirstOrDefault(p => p.name.Equals(Name, StringComparison.InvariantCultureIgnoreCase));
                        if (found != null) { // Alvo encontrado, atualizar
                            UpdateFromSwapi(found);
                            return;
                        }
                        // Alvo não encontrado, ler próximo "lote"
                        request = result.next;
                    } while (!string.IsNullOrEmpty(request)); // Parar se a próxima página for null
                }
                catch (System.Net.WebException) {
                    return;
                }
            }
        }

        /// <summary>
        /// Procura um planeta no swapi por id
        /// </summary>
        /// <param name="web">O cliente Web aberto</param>
        /// <param name="url">A url do planeta, por ex. https://swapi.co/api/planets/1</param>
        /// <returns></returns>
        private bool GetSwapiPlanet(WebClient web, string url) {
            var json = web.DownloadString(url);
            var found = JsonConvert.DeserializeObject<PlanetSwapi>(json);
            if (found == null) {
                return false;
            }
            UpdateFromSwapi(found);
            return true;
        }

        /// <summary>
        /// Atualiza plante no nosso BD com os dados do planeta correspondente no swapi
        /// Name, Movies e SwapId serão sempre do swapi
        /// Climate e Terrain poderão ser substituídos pelo usuário
        /// </summary>
        /// <param name="found">Planeta encontrado no swapi</param>
        private void UpdateFromSwapi(PlanetSwapi found) {
            Name = found.name;
            Climate = string.IsNullOrWhiteSpace(Climate) ? found.climate : Climate;
            Terrain = string.IsNullOrWhiteSpace(Terrain) ? found.terrain : Terrain;
            Movies = found.films.Count;
            SwapiId = int.Parse(found.url.Split('/').LastOrDefault(p => !string.IsNullOrEmpty(p)));
        }
    }

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

        // Propriedades não utilizadas
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