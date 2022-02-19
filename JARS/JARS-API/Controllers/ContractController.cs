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
    [Route("api/v1/contracts")]
    public class ContractController : ControllerBase
    {
        private readonly IContractRepository _repository;
        private readonly IBillRepository _billRepository;
        private readonly INoteRepository _noteRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ContractController(IContractRepository repository, IBillRepository billRepository,
            INoteRepository noteRepository, ICategoryRepository categoryRepository)
        {
            _repository = repository;
            _billRepository = billRepository;
            _noteRepository = noteRepository;
            _categoryRepository = categoryRepository;
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
                Note note = new Note();
                Bill bill = new Bill();

                if (contract.Note != null)
                {
                    note = new Note
                    {
                        AddedDate = contract.Note.AddedDate,
                        Comments = contract.Note.Comments,
                        Image = contract.Note.Image,
                    };
                    await _noteRepository.Add(note);
                }

                Contract _contract = new Contract
                {
                    StartDate = contract.StartDate,
                    EndDate = contract.EndDate,
                    Amount = contract.Amount,
                    NoteId = note.Id > 0 ? note.Id : null,
                    AccountId = GetCurrentUID(),
                };
                await _repository.CreateContractAsync(_contract);

                var createdContract = _repository.GetContractByContractIdAsync(_contract.Id, GetCurrentUID());
                Note _note = new Note
                {
                    Id = note.Id,
                    AddedDate = note.AddedDate,
                    Comments = note.Comments,
                    Image = note.Image,
                    ContractId = createdContract != null ? createdContract.Id : null,
                };
                await _noteRepository.Update(_note);
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
                    Amount = contract.Amount == null ? result.Amount : result.Amount + contract.Amount,
                    AccountId = result.AccountId,
                };
                await _repository.UpdateContractAsync(_contract);

                if (contract.Amount != null)
                {
                    var contractBills = await _billRepository.GetAllBillByContractIdAsync(id, GetCurrentUID());
                    foreach (var bill in contractBills)
                    {
                        if (bill.LeftAmount > 0)
                        {
                            Bill updatedBill = new Bill
                            {
                                Id = bill.Id,
                                Date = bill.Date,
                                Amount = bill.Amount + contract.Amount,
                                LeftAmount = bill.LeftAmount + contract.Amount,
                                Name = bill.Name,
                                ContractId = bill.ContractId,
                                CategoryId = bill.CategoryId,
                            };
                            await _billRepository.UpdateBillAsync(updatedBill);
                        }
                    }
                }               
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
