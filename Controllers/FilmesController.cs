using FilmesAPI.Data;
using FilmesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmesAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class FilmesController : ControllerBase {
        private readonly AppDbContext _context;

        public FilmesController(AppDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Filme>>> Get() =>
            await _context.Filmes.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Filme>> Get(int id) {
            var filme = await _context.Filmes.FindAsync(id);
            return filme == null ? NotFound() : Ok(filme);
        }

        [HttpPost]
        public async Task<ActionResult<Filme>> Post(Filme filme) {
            _context.Filmes.Add(filme);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = filme.Id }, filme);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Filme filme) {
            if (id != filme.Id) return BadRequest();
            _context.Entry(filme).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) {
            var filme = await _context.Filmes.FindAsync(id);
            if (filme == null) return NotFound();
            _context.Filmes.Remove(filme);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("lote")]
        public async Task<ActionResult> PostEmLote(List<Filme> filmes) {
            foreach (var filme in filmes) {
                // Força o DateTime como UTC
                filme.DataLancamento = DateTime.SpecifyKind(filme.DataLancamento, DateTimeKind.Utc);
            }

            _context.Filmes.AddRange(filmes);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = $"{filmes.Count} filmes adicionados com sucesso!" });
        }
    }
}
