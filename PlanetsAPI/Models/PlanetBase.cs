// Para uso do MongoDB
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace PlanetsAPI.Models {
    public abstract class PlanetBase {
        private const string swapi = @"https://swapi.co/api/planets";

        // O campo Id é definido na classe: long para o "in memory" e string para o MongoDb

        /// <summary>
        /// Nome do planeta - fornecido pelo usuário ou obtido no swapi
        /// </summary>
        [BsonElement("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Clima do planeta - fornecido pelo usuário ou obtido no swapi
        /// </summary>
        [BsonElement("Climate")]
        public string Climate { get; set; }  // Fornecido pelo usuário ou obtido no swapi

        /// <summary>
        /// Tipo de terreno do planeta - fornecido pelo usuário ou obtido no swapi
        /// </summary>
        [BsonElement("Terrain")]
        public string Terrain { get; set; }  // Fornecido pelo usuário ou obtido no swapi

        /// <summary>
        /// Número de filmes onde o planeta aparece - obtido no swapi se for planeta Star Wars, senão 0
        /// </summary>
        [BsonElement("Movies")]
        public int Movies { get; private set; }

        /// <summary>
        /// Id do planeta no swapi - Obtido no swapi se for planeta Star Wars, senão 0
        /// </summary>
        [BsonElement("SwapiId")]
        public int SwapiId { get; set; }

        /// <summary>
        /// URL completa para o planeta no swapi - calculado
        /// </summary>
        public string SwapiURL => IsStarWars ? $"{swapi}/{SwapiId}/" : null;  // Para facilitar as buscas, não é salvo no BD

        /// <summary>
        /// Planet aparece em filmes Star Wars?
        /// </summary>
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
        /// <returns>true se o planeta estiver no swapi, false caso contrário</returns>
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
}