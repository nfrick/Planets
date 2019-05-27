using Microsoft.AspNetCore.Mvc;
using PlanetsAPI.Models;
using PlanetsAPI.Services;
using System.Collections.Generic;

namespace PlanetsAPI.Controllers {
    [Route("api/planetm")]
    [ApiController]
    public class PlanetMController : ControllerBase {

        private readonly PlanetMService _planetMService;

        public PlanetMController(PlanetMService planetMService) {
            _planetMService = planetMService;
        }

        [HttpGet]
        public ActionResult<List<PlanetM>> GetPlanets() {
            return _planetMService.Get();
        }

        [HttpGet("{id:length(24)}", Name = "GetPlanet")]
        public ActionResult<PlanetM> GetPlanet(string id) {
            var planet = _planetMService.Get(id);

            if (planet == null) {
                return NotFound();
            }

            return planet;
        }

        [HttpPost]
        public ActionResult<PlanetM> PostPlanet(PlanetM planet) {
            if (_planetMService.Create(planet)) {
                return CreatedAtRoute("GetPlanet", new { id = planet.Id }, planet);
            }
            else {
                return NotFound();
            }
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult PutPlanet(string id, PlanetM planet) {
            var p = _planetMService.Get(id);
            if (p == null) {
                return NotFound();
            }
            if (_planetMService.Update(id, planet)) {
                return NoContent();
            }
            else {
                return NotFound();
            }
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult DeletePlanet(string id) {
            var planet = _planetMService.Get(id);
            if (planet == null) {
                return NotFound();
            }
            _planetMService.Remove(planet.Id);
            return NoContent();
        }
    }
}