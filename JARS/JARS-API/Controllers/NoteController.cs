#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace JARS_API.Controllers
{
    [Route("api/v1/notes")]
    [Authorize]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly INoteRepository _noteRepository;
        private readonly IContractRepository _contractRepository;
        private readonly ITransactionRepository _transactionRepository;

        public NoteController(INoteRepository noteRepository, IContractRepository contractRepository, 
            ITransactionRepository transactionRepository)
        {
            _noteRepository = noteRepository;
            _contractRepository = contractRepository;
            _transactionRepository = transactionRepository;
        }
        // GET: api/Note
        // [HttpGet]
        // public async Task<IEnumerable<Note>> GetTransactions()
        // {
        //     return await noteRepository.GetNotes();
        // }

        // GET: api/Note/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNote(int id)
        {
            var note = await _noteRepository.GetNote(id);
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
            var result = await _noteRepository.GetNote(id);
            if (result == null)
            {
                return BadRequest();
            }
            try
            {
                Note _note = new Note
                {
                    Id = result.Id,
                    AddedDate = note.AddedDate == null ? result.AddedDate : note.AddedDate,
                    Comments = note.Comments == null ? result.Comments : note.Comments,
                    Image = note.Image == null ? result.Image : note.Image,
                    Latitude = note.Latitude == null ? result.Latitude : note.Latitude,
                    Longitude = note.Longitude == null ? result.Longitude : note.Longitude,
                };
                await _noteRepository.Update(_note);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_noteRepository.GetNote(note.Id) == null)
                {
                    return NotFound();
                }
                else { throw; }
            }
            return Ok(note);
        }

        // POST: api/Note
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("contract-note")]
        public async Task<ActionResult<Note>> PostNoteForContract([FromQuery]int contract_id, Note note)
        {
            try
            {
                Console.WriteLine(contract_id);
                var contract = await _contractRepository.GetContractByContractIdAsync(contract_id, GetCurrentUID());
                var result = await _noteRepository.GetNoteByContractId(contract_id);
                if (result == null && contract != null)
                {
                    Note _note = new Note
                    {
                        AddedDate = note.AddedDate,
                        Comments = note.Comments,
                        Image = note.Image,
                        Latitude = note.Latitude,
                        Longitude = note.Longitude,
                        ContractId = contract_id
                    };
                    await _noteRepository.Add(_note);

                    var createdNote = await _noteRepository.GetNote(_note.Id);
                    if (createdNote != null)
                    {
                        Contract _contract = new Contract
                        {
                            Id = contract.Id,
                            AccountId = contract.AccountId,
                            ScheduleTypeId = contract.ScheduleTypeId,
                            NoteId = contract.NoteId == null ? createdNote.Id : contract.NoteId,
                            StartDate = contract.StartDate,
                            EndDate = contract.EndDate,
                            Amount = contract.Amount,
                            Name = contract.Name,
                        };
                        await _contractRepository.UpdateContractAsync(_contract);
                    }
                } else
                {
                    return BadRequest();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return CreatedAtAction("GetNote", new { id = note.Id }, note);
        }

        // POST: api/Note
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("transaction-note")]
        public async Task<ActionResult<Note>> PostNoteForTransaction([FromQuery] int transaction_id, Note note)
        {
            try
            {
                var transaction = await _transactionRepository.GetTransaction(transaction_id, GetCurrentUID());
                var result = await _noteRepository.GetNoteByTransactionId(transaction_id);

                if (result == null && transaction != null)
                {
                    Note _note = new Note
                    {
                        AddedDate = note.AddedDate,
                        Comments = note.Comments,
                        Image = note.Image,
                        TransactionId = transaction_id
                    };
                    await _noteRepository.Add(_note);

                    var createdNote = await _noteRepository.GetNote(_note.Id);
                    if (createdNote != null)
                    {
                        Transaction _transaction = new Transaction
                        {
                            Id = transaction.Id,
                            WalletId = transaction.Id,
                            TransactionDate = transaction.TransactionDate,
                            BillId = transaction.BillId,
                            Amount = transaction.Amount,
                            NoteId = transaction.NoteId == null ? createdNote.Id : transaction.NoteId,
                        };
                        await _transactionRepository.Update(_transaction);
                    }
                }
                else
                {
                    return BadRequest();
                }
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
                await _noteRepository.Delete(note);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(note);
        }

        private string GetCurrentUID()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
