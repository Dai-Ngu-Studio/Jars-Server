#nullable disable
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;
using JARS_API.BusinessModels;

namespace JARS_API.Controllers
{
    [Route("api/v1/transactions")]
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

        [HttpGet("weekly")]
        public async Task<IEnumerable<TransactionWeekly>> GetReportWeeklyTransactions()
        {
            decimal expenseWeek1Ago = 0;
            decimal expenseWeek2Ago = 0;
            decimal expenseWeek3Ago = 0;
            decimal expenseWeek4Ago = 0;
            decimal incomeWeek1Ago = 0;
            decimal incomeWeek2Ago = 0;
            decimal incomeWeek3Ago = 0;
            decimal incomeWeek4Ago = 0;
            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;

            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            IEnumerable<Transaction> transactionsWeek1 = new JarsDatabaseContext().Transactions
                .Include(x => x.Wallet)
                .ThenInclude(x => x.Account)
                .Where(t => t.Wallet!.Account!.Id == GetCurrentUID()).ToList();
            transactionsWeek1 = transactionsWeek1.Where(x =>
                myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW) -
                myCal.GetWeekOfYear((DateTime) (x.TransactionDate), myCWR, myFirstDOW) == 0);
            foreach (var transaction in transactionsWeek1)
            {
                if (transaction.Amount > 0)
                {
                    incomeWeek1Ago += (decimal) (transaction.Amount);
                }
                else expenseWeek1Ago -= (decimal) (transaction.Amount);
            }

            IEnumerable<Transaction> transactionsWeek2 = new JarsDatabaseContext().Transactions
                .Where(t => t.Wallet!.Account!.Id == GetCurrentUID()).ToList();
            transactionsWeek2 = transactionsWeek2.Where(x =>
                myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW) -
                myCal.GetWeekOfYear((DateTime) (x.TransactionDate), myCWR, myFirstDOW) == 1);
            foreach (var transaction in transactionsWeek2)
            {
                if (transaction.Amount > 0)
                {
                    incomeWeek1Ago += (decimal) (transaction.Amount);
                }
                else expenseWeek1Ago -= (decimal) (transaction.Amount);
            }

            IEnumerable<Transaction> transactionsWeek3 = new JarsDatabaseContext().Transactions
                .Where(t => t.Wallet!.Account!.Id == GetCurrentUID()).ToList();
            transactionsWeek3 = transactionsWeek3.Where(x =>
                myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW) -
                myCal.GetWeekOfYear((DateTime) (x.TransactionDate), myCWR, myFirstDOW) == 2);
            foreach (var transaction in transactionsWeek3)
            {
                if (transaction.Amount > 0)
                {
                    incomeWeek1Ago += (decimal) (transaction.Amount);
                }
                else expenseWeek1Ago -= (decimal) (transaction.Amount);
            }

            IEnumerable<Transaction> transactionsWeek4 = new JarsDatabaseContext().Transactions
                .Where(t => t.Wallet!.Account!.Id == GetCurrentUID()).ToList();
            transactionsWeek4 = transactionsWeek4.Where(x =>
                myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW) -
                myCal.GetWeekOfYear((DateTime) (x.TransactionDate), myCWR, myFirstDOW) == 3);
            foreach (var transaction in transactionsWeek4)
            {
                if (transaction.Amount > 0)
                {
                    incomeWeek1Ago += (decimal) (transaction.Amount);
                }
                else expenseWeek1Ago -= (decimal) (transaction.Amount);
            }

            List<TransactionWeekly> transactionsWeeklys = new List<TransactionWeekly>();

            transactionsWeeklys.Add(new TransactionWeekly
            {
                income = incomeWeek1Ago,
                expense = expenseWeek1Ago,
            });
            transactionsWeeklys.Add(new TransactionWeekly
            {
                income = incomeWeek2Ago,
                expense = expenseWeek2Ago,
            });
            transactionsWeeklys.Add(new TransactionWeekly
            {
                income = incomeWeek2Ago,
                expense = expenseWeek2Ago,
            });
            transactionsWeeklys.Add(new TransactionWeekly
            {
                income = incomeWeek2Ago,
                expense = expenseWeek2Ago,
            });
            return transactionsWeeklys;
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
                else
                {
                    throw;
                }
            }

            return Ok(transaction);
        }

        // POST: api/Transaction
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("income")]
        public async Task<ActionResult> PostTransactionIncome(IncomeTransaction incomeTransaction)
        {
            try
            {
                JarsDatabaseContext context = new JarsDatabaseContext();
                var id = GetCurrentUID();
                var wallets = context.Wallets.Where(x => x.AccountId == id).ToList();
                foreach (var wallet in wallets)
                {
                    var amount = incomeTransaction.Amount * (wallet.Percentage / 100);
                    wallet.WalletAmount += amount;
                    Note note = null;
                    if (!string.IsNullOrEmpty(incomeTransaction.NoteComment) ||
                        !string.IsNullOrEmpty(incomeTransaction.NoteImage))
                    {
                        note = new Note
                        {
                            Comments = incomeTransaction.NoteComment,
                            AddedDate = DateTime.Now,
                            Image = incomeTransaction.NoteImage,
                        };
                        context.Notes.Add(note);
                        context.SaveChanges();
                    }


                    Transaction transaction = new Transaction
                    {
                        Amount = amount,
                        WalletId = wallet.Id,
                        NoteId = note != null ? note.Id : null,
                        TransactionDate = DateTime.Now
                    };
                    context.Transactions.Add(transaction);
                    context.SaveChanges();
                    if (note != null)
                    {
                        note.TransactionId = transaction.Id;
                        context.Notes.Update(note);
                        context.SaveChanges();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok();
        }

        [HttpPost("expense")]
        public async Task<ActionResult> PostTransactionExpense(ExpenseTransaction expenseTransaction)
        {
            try
            {
                JarsDatabaseContext context = new JarsDatabaseContext();
                var id = GetCurrentUID();
                var wallet =
                    context.Wallets.FirstOrDefault(x => x.AccountId == id && x.Id == expenseTransaction.WalletId);
                if (wallet == null)
                {
                    return BadRequest();
                }

                if (wallet.WalletAmount < expenseTransaction.Amount)
                {
                    return BadRequest(new
                    {
                        msg = "Not enough money"
                    });
                }
                if (expenseTransaction.Amount > 0)
                {
                    return BadRequest(new 
                    {
                        msg = "Expense amount must be less than zero"
                    });
                }
                wallet.WalletAmount += expenseTransaction.Amount;
                Note note = null;
                if (!string.IsNullOrEmpty(expenseTransaction.NoteComment) ||
                    !string.IsNullOrEmpty(expenseTransaction.NoteImage))
                {
                    note = new Note
                    {
                        Comments = expenseTransaction.NoteComment,
                        AddedDate = DateTime.Now,
                        Image = expenseTransaction.NoteImage,
                    };
                    context.Notes.Add(note);
                    context.SaveChanges();
                }
                
                Transaction transaction = new Transaction
                {
                    Amount = expenseTransaction.Amount,
                    WalletId = wallet.Id,
                    NoteId = note != null ? note.Id : null,
                    TransactionDate = DateTime.Now
                };
                context.Transactions.Add(transaction);
                context.SaveChanges();
                if (note != null)
                {
                    note.TransactionId = transaction.Id;
                    context.Notes.Update(note);
                    context.SaveChanges();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok();
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
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}