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
    [Route("api/v1/[controller]")]
    public class ContractController : ControllerBase
    {
        private readonly IContractRepository _repository;

        public ContractController(IContractRepository repository)
        {
            _repository = repository;
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
                Contract _contract = new Contract
                {
                    AccountId = GetCurrentUID(),
                    StartDate = contract.StartDate,
                    EndDate = contract.EndDate,
                    Amount = contract.Amount,
                };
                await _repository.CreateContractAsync(_contract);
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
                    StartDate = contract.StartDate,
                    EndDate = contract.EndDate,
                    Amount = contract.Amount,
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
