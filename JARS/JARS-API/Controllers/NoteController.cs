#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JARS_DAL.Models;
using JARS_DAL.Repository;
namespace JARS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        INoteRepository noteRepository = new NoteRepository();

        // GET: api/Note
        [HttpGet]
        public async Task<IEnumerable<Note>> GetTransactions()
        {
            return await noteRepository.GetNotes();
        }

        // GET: api/Note/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNote(int id)
        {
            var note = await noteRepository.GetNote(id);
            if (note == null)
            {
                return NotFound();
            }

            return note;
        }

        // PUT: api/Note/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNote(int id, Note note)
        {
            if (id != note.Id)
            {
                return BadRequest();
            }
            try
            {
                await noteRepository.Update(note);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (noteRepository.GetNote(note.Id) == null)
                {
                    return NotFound();
                }
                else { throw; }
            }
            return Ok(note);
        }

        // POST: api/Note
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Note>> PostNote(Note note)
        {
            try
            {
                await noteRepository.Add(note);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return CreatedAtAction("GetNote", new { id = note.Id }, note);
        }

        // DELETE: api/Note/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            Note note = new Note
            {
                Id = id
            };
            try
            {
                await noteRepository.Delete(note);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(note);
        }
    }
}
