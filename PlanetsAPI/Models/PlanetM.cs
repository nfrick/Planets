// Para uso do MongoDB
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PlanetsAPI.Models {
    public class PlanetM : PlanetBase {

        /// <summary>
        /// Id do planeta - gerado pelo banco de dados
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}