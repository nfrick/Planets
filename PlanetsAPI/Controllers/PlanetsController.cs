using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanetsAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;

namespace PlanetsApi.Controllers {
    [Route("api/planet")]
    [ApiController]
    public class PlanetController : ControllerBase {
        private readonly PlanetContext _context;

        public PlanetController(PlanetContext context) {
            _context = context;

            // Criar um planeta no caso da lista estar vazia
            // Foi útil durante os testes
            //if (_context.Planets.Any()) {
            //    return;
            //}
            //var planet = new Planet { Name = "1" };
            //planet.SyncWithSwapi();
            //_context.Planets.Add(planet);
            //_context.SaveChanges();
        }

        // GET: api/Planet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Planet>>> GetPlanets() {
            return await _context.Planets.ToListAsync();
        }

        // GET: api/Planet/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Planet>> GetPlanet(long id) {
            var planet = await _context.Planets.FindAsync(id);
            if (planet == null) {
                return NotFound();
            }
            return planet;
        }


        // POST: api/Planet
        [HttpPost]
        public async Task<ActionResult<Planet>> PostPlanet(Planet planet) {
            // Obter os dados do swapi antes de salvar
            planet.SyncWithSwapi();

            // Verificar se planeta com este nome já existe
            if (_context.Planets.Any(p => p.Name.Equals(planet.Name, StringComparison.InvariantCultureIgnoreCase))) {
                return BadRequest();
            }

            _context.Planets.Add(planet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlanet), new { id = planet.Id }, planet);
        }

        // PUT: api/Planet/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlanet(long id, Planet planet) {

            // Náo salvar caso: 1) Id incorreto ou 2) Já existe outro planeta com este nome
            if (id != planet.Id ||
                _context.Planets.Any(p => p.Id != id && p.Name.Equals(planet.Name, StringComparison.InvariantCultureIgnoreCase))) {
                return BadRequest();
            }
            planet.SyncWithSwapi();
            _context.Entry(planet).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Planet/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id) {
            var planet = await _context.Planets.FindAsync(id);

            if (planet == null) {
                return NotFound();
            }

            _context.Planets.Remove(planet);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}