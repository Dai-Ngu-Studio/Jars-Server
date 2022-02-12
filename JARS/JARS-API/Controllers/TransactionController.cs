#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JARS_DAL.Models;
using JARS_DAL.Repository;
namespace JARS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        ITransactionRepository _transactionRep = new TransactionRepository();

        // GET: api/Transaction
        [HttpGet]
        public async Task<IEnumerable<Transaction>> GetTransactions()
        {
            return await _transactionRep.GetTransactions();
        }

        // GET: api/Transaction/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _transactionRep.GetTransaction(id);
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
                if (_transactionRep.GetTransaction(transaction.Id) == null)
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
                await _transactionRep.Delete(transaction);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(transaction);
        }
    }
}
