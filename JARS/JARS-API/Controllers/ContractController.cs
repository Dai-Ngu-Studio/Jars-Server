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

        public ContractController(IContractRepository repository, IBillRepository billRepository,
            INoteRepository noteRepository)
        {
            _repository = repository;
            _billRepository = billRepository;
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
                Note note = new Note();
                Bill bill = new Bill();

                if (contract.Note != null)
                {
                    note = new Note
                    {
                        AddedDate = contract.Note.AddedDate,
                        Comments = contract.Note.Comments,
                        Image = contract.Note.Image,
                        Latitude = contract.Note.Latitude,
                        Longitude = contract.Note.Longitude,
                    };
                    await _noteRepository.Add(note);
                }

                Contract _contract = new Contract
                {
                    StartDate = contract.StartDate,
                    EndDate = contract.EndDate,
                    Amount = contract.Amount,
                    Name = contract.Name,
                    ScheduleTypeId = contract.ScheduleTypeId,
                    NoteId = note.Id > 0 ? note.Id : null,
                    AccountId = GetCurrentUID(),
                };
                await _repository.CreateContractAsync(_contract);

                var createdContract = await _repository.GetContractByContractIdAsync(_contract.Id, GetCurrentUID());
                var createdNote = await _noteRepository.GetNote(note.Id);
                if (createdContract != null && createdNote != null)
                {
                    Note _note = new Note
                    {
                        Id = note.Id,
                        AddedDate = note.AddedDate,
                        Comments = note.Comments,
                        Image = note.Image,
                        ContractId = createdContract != null ? createdContract.Id : null,
                        Latitude = note.Latitude,
                        Longitude = note.Longitude,
                    };
                    await _noteRepository.Update(_note);
                }

                if (contract.StartDate == null)
                    return BadRequest();
                else
                {
                    if (DateTime.Compare(DateTime.Today, contract.StartDate.Value.Date) == 0)
                    {
                        bill = new Bill
                        {
                            Date = _contract.StartDate,
                            Name = _contract.Name,
                            ContractId = _contract.Id,
                            Amount = _contract.Amount,
                            LeftAmount = _contract.Amount,
                        };
                        await _billRepository.CreateBillAsync(bill);
                    }
                }
                return CreatedAtAction("GetContract", new { id = contract.Id }, _contract);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }          
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateContract(int id, Contract contract)
        {
            var result = await _repository.GetContractByContractIdAsync(id, GetCurrentUID());
            
            if (result == null)
            {
                return BadRequest();
            }
            try
            {
                decimal? amount = 0;
                if (contract.Amount > 0)
                {
                    amount = result.Amount + contract.Amount;
                } else
                {
                    amount = result.Amount + contract.Amount;
                    if (amount < 0)
                        amount = 0;
                }

                Contract _contract = new Contract
                {
                    Id = id,
                    StartDate = contract.StartDate == null ? result.StartDate : contract.StartDate,
                    EndDate = contract.EndDate == null ? result.EndDate : contract.EndDate,
                    Amount = contract.Amount == null ? result.Amount : amount,
                    Name = contract.Name == null ? result.Name : contract.Name,
                    AccountId = result.AccountId,
                };
                await _repository.UpdateContractAsync(_contract);

                if (contract.Amount != null)
                {
                    var contractBills = await _billRepository.GetAllBillByContractIdAsync(id);
                    if (contractBills != null)
                    {
                        foreach (var bill in contractBills)
                        {
                            if (bill.LeftAmount > 0)
                            {
                                decimal? leftAmount = 0;
                                decimal? billAmount = 0;
                                if (contract.Amount > 0)
                                {
                                    billAmount = bill.Amount + contract.Amount;
                                    leftAmount = bill.LeftAmount + contract.Amount;
                                }
                                else
                                {
                                    billAmount = bill.Amount + contract.Amount;
                                    leftAmount = bill.LeftAmount + contract.Amount;
                                    if (leftAmount < 0)
                                        leftAmount = 0;
                                    if (billAmount < 0)
                                        billAmount = 0;
                                }

                                Bill updatedBill = new Bill
                                {
                                    Id = bill.Id,
                                    Date = bill.Date,
                                    Amount = billAmount,
                                    LeftAmount = leftAmount,
                                    Name = bill.Name,
                                    ContractId = bill.ContractId,
                                    CategoryId = bill.CategoryId,
                                };
                                await _billRepository.UpdateBillAsync(updatedBill);
                            }
                        }
                    }                 
                }
                return CreatedAtAction("GetContract", new { id = contract.Id }, _contract);
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
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
