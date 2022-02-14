#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace JARS_API.Controllers
{
    [Route("api/v1/[controller]s")]
    [Authorize]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private ITransactionRepository _transactionRep;
        public TransactionController(ITransactionRepository repository)
        {
            _transactionRep = repository;
        }


        // GET: api/Transaction
        [HttpGet]
        public async Task<IEnumerable<Transaction>> GetTransactions()
        {

            return await _transactionRep.GetTransactions(GetCurrentUID());
        }

        // GET: api/Transaction/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            ClaimsPrincipal httpUser = HttpContext.User as ClaimsPrincipal;
            string uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var transaction = await _transactionRep.GetTransaction(id, GetCurrentUID());
            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        // PUT: api/Transaction/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(int id, Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return BadRequest();
            }
            try
            {
                await _transactionRep.Update(transaction);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_transactionRep.GetTransaction(transaction.Id, GetCurrentUID()) == null)
                {
                    return NotFound();
                }
                else { throw; }
            }
            return Ok(transaction);
        }

        // POST: api/Transaction
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
            try
            {
                await _transactionRep.Add(transaction);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return CreatedAtAction("GetTransaction", new { id = transaction.Id }, transaction);
        }

        // DELETE: api/Transaction/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            Transaction transaction = new Transaction
            {
                Id = id
            };
            try
            {
                await _transactionRep.Delete(transaction, GetCurrentUID());
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(transaction);
        }

        private string GetCurrentUID()
        {
            ClaimsPrincipal httpUser = HttpContext.User as ClaimsPrincipal;
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
