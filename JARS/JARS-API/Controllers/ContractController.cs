using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Mvc;

namespace JARS_API.Controllers
{
    [ApiController]
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
            var result = await _repository.GetAllContractAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contract>> GetContract(int id)
        {
            var contract = await _repository.GetContractByContractIdAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            return Ok(contract);
        }
    }
}
