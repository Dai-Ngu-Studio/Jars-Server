using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JARS_API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]s")]
    public class ContractController : ControllerBase
    {
        private readonly IContractRepository _repository;
        private readonly INoteRepository _noteRepository;

        public ContractController(IContractRepository repository, INoteRepository noteRepository)
        {
            _repository = repository;
            _noteRepository = noteRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Contract>>> GetAllContracts()
        {
            var result = await _repository.GetAllContractAsync(GetCurrentUID());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contract>> GetContract(int id)
        {
            var contract = await _repository.GetContractByContractIdAsync(id, GetCurrentUID());
            if (contract == null)
            {
                return NotFound();
            }

            return Ok(contract);
        }

        [HttpPost]
        public async Task<ActionResult> CreateContract(Contract contract)
        {
            try
            {
                if (contract.Note == null)
                {
                    Contract _contract = new Contract
                    {
                        AccountId = GetCurrentUID(),
                        StartDate = contract.StartDate,
                        EndDate = contract.EndDate,
                        Amount = contract.Amount,
                    };
                    await _repository.CreateContractAsync(_contract);
                } else
                {
                    Note note = new Note
                    {
                        AddedDate = contract.Note.AddedDate,
                        Comments = contract.Note.Comments,
                        Image = contract.Note.Image,
                    };
                    await _noteRepository.Add(note);

                    Contract _contract = new Contract
                    {
                        StartDate = contract.StartDate,
                        EndDate = contract.EndDate,
                        Amount = contract.Amount,
                        NoteId = note.Id,
                        AccountId = GetCurrentUID(),
                    };
                    await _repository.UpdateContractAsync(_contract);
                }                
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return CreatedAtAction("GetContract", new { id = contract.Id }, contract);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateContract(int id, Contract contract)
        {
            var result = await _repository.GetContractByContractIdAsync(id, GetCurrentUID());
            
            if (result.Id < 0)
            {
                return BadRequest();
            }
            try
            {
                Contract _contract = new Contract
                {
                    Id = id,
                    StartDate = contract.StartDate == null ? result.StartDate : contract.StartDate,
                    EndDate = contract.EndDate == null ? result.EndDate : contract.EndDate,
                    Amount = contract.Amount == null ? result.Amount : contract.Amount,
                    AccountId = result.AccountId,
                };
                await _repository.UpdateContractAsync(_contract);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_repository.GetContractByContractIdAsync(contract.Id, GetCurrentUID()) == null)
                {
                    return NotFound();
                }
                else { throw; }
            }
            return Ok(contract);
        }

        private string GetCurrentUID()
        {
            ClaimsPrincipal httpUser = HttpContext.User as ClaimsPrincipal;
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
