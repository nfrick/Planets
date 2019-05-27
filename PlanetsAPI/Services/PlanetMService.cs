using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using PlanetsAPI.Models;
using System.Collections.Generic;
using System.Linq;


namespace PlanetsAPI.Services {
    public class PlanetMService {
        private readonly IMongoCollection<PlanetM> _planets;

        public PlanetMService(IConfiguration config) {
            var client = new MongoClient(config.GetConnectionString("PlanetsDb"));
            var database = client.GetDatabase("PlanetsDb");
            _planets = database.GetCollection<PlanetM>("Planets");
        }

        /// <summary>
        /// Lista de todos os planetas do banco de dados
        /// </summary>
        /// <returns>List<PlanetM></returns>
        public List<PlanetM> Get() {
            return _planets.Find(p => true).ToList();
        }

        /// <summary>
        /// Encontra planeta por Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>PlanetM</returns>
        public PlanetM Get(string id) {
            return _planets.Find<PlanetM>(p => p.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Cria novo planeta
        /// </summary>
        /// <param name="planet"></param>
        /// <returns>true se planeta for criado, false se já existir</returns>
        public bool Create(PlanetM planet) {
            planet.SyncWithSwapi();
            if (IsPlanetInvalid(planet)) {
                return false;
            }

            _planets.InsertOne(planet);
            return true;
        }

        /// <summary>
        /// Atualiza planeta no banco de dados
        /// </summary>
        /// <param name="id"></param>
        /// <param name="planet"></param>
        public bool Update(string id, PlanetM planet) {
            planet.SyncWithSwapi();
            if (IsPlanetInvalid(planet)) {
                return false;
            }

            _planets.ReplaceOne(p => p.Id == id, planet);
            return true;
        }

        /// <summary>
        /// Remove planeta do banco de dados
        /// </summary>
        /// <param name="planet">Objeto PlanetM</param>
        public void Remove(PlanetM planet) {
            _planets.DeleteOne(p => p.Id == planet.Id);
        }

        /// <summary>
        /// Remove planeta do banco de dados
        /// </summary>
        /// <param name="id">Id do planeta a ser deletado</param>
        public void Remove(string id) {
            _planets.DeleteOne(p => p.Id == id);
        }

        /// <summary>
        /// Valida o planeta
        /// Nome não pode estar em branco e não pode existir outro planeta com mesmo nome
        /// Opcional: invalidar se não for Star Wars
        /// </summary>
        /// <param name="planet"></param>
        /// <returns>true se o Name for INVÁLIDO</returns>
        public bool IsPlanetInvalid(PlanetM planet) {
            // Para permitir apenas planetas Star Wars, comentar as linhas abaixo
            /*return string.IsNullOrWhiteSpace(planet.Name) || // Nome em branco
                     _planets.Find(p => p.Id != planet.Id &&   // Não existir outro planeta com mesmo nome
                     p.Name.ToLower() == planet.Name.ToLower()).Any(); */

            // Para permitir qualquer planeta, comentar as linhas abaixo
            return string.IsNullOrWhiteSpace(planet.Name) || // Nome em branco
                   !planet.IsStarWars ||  // Planeta não é Star Wars
                   _planets.Find(p => p.Id != planet.Id &&   // Não existir outro planeta com mesmo nome
                                      p.Name.ToLower() == planet.Name.ToLower()).Any();
        }
    }
}