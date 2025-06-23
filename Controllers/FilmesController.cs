using FilmesAPI.Data;
using FilmesAPI.Models;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Retorna todos os filmes e séries cadastrados no sistema
        /// </summary>
        /// <returns>Lista de Filmes e Séries</returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Filme>>> Get() =>
            await _context.Filmes.ToListAsync();

        /// <summary>
        /// Retorna um filme ou série baseado no ID recebido
        /// </summary>
        /// <param name="id">ID do filme/série</param>
        /// <returns>Informações sobre o filme/série correspondente ao ID</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Filme>> Get(int id) {
            var filme = await _context.Filmes.FindAsync(id);
            return filme == null ? NotFound() : Ok(filme);
        }

        /// <summary>
        /// Adiciona um novo filme/série ao sistema
        /// </summary>
        /// <param name="filme">Objeto com as informações do filme/série</param>
        /// <returns>Filme/Série cadastrada no sistema</returns>
        [HttpPost]
        public async Task<ActionResult<Filme>> Post(Filme filme) {
            _context.Filmes.Add(filme);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = filme.Id }, filme);
        }

        /// <summary>
        /// Atualiza um filme/série presente no sistema
        /// </summary>
        /// <param name="id">ID do filme/série</param>
        /// <param name="filme">Objeto com as informações do filme/série</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Filme filme) {
            if (id != filme.Id) return BadRequest();
            _context.Entry(filme).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Remove um filme/série no sistema
        /// </summary>
        /// <param name="id">ID do filme/série a ser removido</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) {
            var filme = await _context.Filmes.FindAsync(id);
            if (filme == null) return NotFound();
            _context.Filmes.Remove(filme);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Insere múltiplos títulos no sistema
        /// </summary>
        /// <param name="filmes">Array de objetos de filmes/séries</param>
        /// <returns>Mensagem de sucesso</returns>
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
