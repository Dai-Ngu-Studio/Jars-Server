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
    [Route("api/v1/bills")]
    public class BillController : ControllerBase
    {
        private readonly IBillRepository _repository;
        private readonly IBillDetailRepository _billDetailRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IWalletReposiotry _walletReposiotry;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICategoryRepository _categoryRepository;

        public BillController(IBillRepository repository, IBillDetailRepository billDetailRepository, IContractRepository contractRepository,
            IWalletReposiotry walletReposiotry, ICategoryRepository categoryRepository, ITransactionRepository transactionRepository)
        {
            _repository = repository;
            _billDetailRepository = billDetailRepository;
            _contractRepository = contractRepository;
            _walletReposiotry = walletReposiotry;
            _categoryRepository = categoryRepository;
            _transactionRepository = transactionRepository;
        }

        /// <summary>
        /// Create bill and create bill details for current UID. Only the owner of the account or admin is authorized to use this method.
        /// </summary>
        /// <param name="bill">Bill in JSON format</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateBill(Bill bill)
        {
            var category = await _categoryRepository.GetCategoryByCategoryIdAsync(bill.CategoryId);
            if (bill.BillDetails.Count > 0)
            {
                Bill _bill = new Bill
                {
                    Date = bill.Date,
                    Name = bill.Name,
                    AccountId = GetCurrentUID(),
                    CategoryId = category != null ? category.Id : null,
                };

                await _repository.CreateBillAsync(_bill);
                foreach (var item in bill.BillDetails)
                {
                    BillDetail billDetail = new BillDetail
                    {
                        ItemName = item.ItemName,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        BillId = _bill.Id,
                    };
                    await _billDetailRepository.CreateBillDetailAsync(billDetail);
                }

                var searchBillDetails = await _billDetailRepository.GetAllBillDetailWithBillIdAsync(_bill.Id);
                var searchBill = await _repository.GetBillByBillIdAsync(_bill.Id, GetCurrentUID());
                decimal? amount = 0;

                if (searchBillDetails != null && searchBill != null)
                {
                    foreach (var item in searchBillDetails)
                    {
                        amount += item.Price * item.Quantity;
                    }
                    _bill = new Bill
                    {
                        Id = searchBill.Id,
                        Date = searchBill.Date,
                        Name = searchBill.Name,
                        LeftAmount = amount,
                        Amount = amount,
                        CategoryId = searchBill.CategoryId,
                        AccountId = searchBill.AccountId,
                    };
                    await _repository.UpdateBillAsync(_bill);
                }
                else
                {
                    return NotFound();
                }
                return CreatedAtAction("GetBill", new { id = bill.Id }, _bill);
            }
            else
            {
                Bill _bill = new Bill
                {
                    Date = bill.Date,
                    Name = bill.Name,
                    AccountId = GetCurrentUID(),
                    CategoryId = category != null ? category.Id : null,
                };
                await _repository.CreateBillAsync(_bill);
                return CreatedAtAction("GetBill", new { id = bill.Id }, _bill);
            }
        }

        /// <summary>
        /// Update bill and create transaction with bill_id, wallet_id and UID. Only the owner of the account or admin is authorized to use this method.
        /// </summary>
        /// <param name="bill_id">bill_id of bill</param>
        /// <param name="wallet_id">wallet_id of the account</param>
        /// <param name="bill">Bill in JSON format</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> UpdateBill([FromQuery]int bill_id, [FromQuery]int wallet_id, Bill bill)
        {
            var result = await _repository.GetBillByBillIdAsync(bill_id, GetCurrentUID());
            if (result == null)
            {
                return BadRequest();
            }
            var walletOwner = await _walletReposiotry.GetWallet(wallet_id);

            if (walletOwner == null)
                return NotFound();
            if (walletOwner.WalletAmount < bill.LeftAmount)
            {
                return BadRequest();
            }

            try
            {
                decimal? leftAmount = 0;
                decimal? transactionLeftAmount = 0;
                if (result.LeftAmount == bill.LeftAmount)
                {
                    leftAmount = result.LeftAmount;
                }
                else
                {
                    if (bill.LeftAmount > 0)
                    {
                        transactionLeftAmount = result.LeftAmount - bill.LeftAmount;

                        if (transactionLeftAmount < 0)
                        {
                            leftAmount = 0;
                            transactionLeftAmount = result.LeftAmount;
                        }
                        else
                        {
                            leftAmount = result.LeftAmount - transactionLeftAmount;
                        }
                    }
                    if (bill.LeftAmount == 0)
                    {
                        leftAmount = 0;
                        transactionLeftAmount = result.Amount;
                    }
                }

                Bill _bill = new Bill
                {
                    Id = result.Id,
                    Date = bill.Date == null ? result.Date : bill.Date,
                    LeftAmount = leftAmount,
                    Name = bill.Name == null ? result.Name : bill.Name,
                    Amount = result.Amount,
                    CategoryId = result.CategoryId,
                    AccountId = result.AccountId,
                    ContractId = result.ContractId,
                };
                await _repository.UpdateBillAsync(_bill);
                
                if (result.LeftAmount != leftAmount)
                {                   
                    Transaction transaction = new Transaction
                    {
                        WalletId = walletOwner.Id,
                        TransactionDate = DateTime.Now,
                        BillId = _bill.Id,
                        Amount = transactionLeftAmount,
                    };
                    await _transactionRepository.Add(transaction);

                    Wallet wallet = new Wallet
                    {
                        Id = walletOwner.Id,
                        Name = walletOwner.Name,
                        WalletAmount = walletOwner.WalletAmount - transaction.Amount < 0 ? 0 : walletOwner.WalletAmount - transaction.Amount,
                        Percentage = walletOwner.Percentage,
                        AccountId = walletOwner.AccountId,
                    };
                    await _walletReposiotry.UpdateWallet(wallet);
                }
                return Ok(_bill);
            }
            catch (DbUpdateConcurrencyException)
            {             
                if (_repository.GetBillByBillIdAsync(bill.Id, GetCurrentUID()) == null)
                {
                    return NotFound();
                }
                else { throw; }
            }         
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBilll(int id)
        {
            Bill bill = new Bill
            {
                Id = id
            };
            try
            {
                var billDetail = await _billDetailRepository.GetAllBillDetailWithBillIdAsync(bill.Id);
                if (billDetail.Count == 0)
                {
                    await _repository.DeleteBillAsync(bill);
                } else
                {
                    return BadRequest("Bill detail still remain for this bill");
                }
                
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(bill);
        }

        [HttpGet("{id}")] 
        public async Task<ActionResult<Bill>> GetBill(int id)
        {
            return await _repository.GetBillByBillIdAsync(id, GetCurrentUID());
        }

        /// <summary>
        /// Get bills for current UID with optional queries. Only the user is authorized to use this method.
        /// </summary>
        /// <param name="page">Parameter "page" is multiplied by the parameter "size" to determine the number of rows to skip. Default value: 0</param>
        /// <param name="size">Maximum number of results to return. Default value: 20</param>
        /// <param name="name">Optional filter for bill's name. Default value: ""</param>
        /// <param name="sortOrder">Optional filter for bill's to sort ascending or descending. Default value: ""</param>
        /// <param name="dateTo">Optional filter for bill's to search from date. Default value: ""</param>
        /// <param name="dateFrom">Optional filter for bill's to search to date. Default value: ""</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Bill>>> GetAllBills(
            [FromQuery] string? name, [FromQuery] string? sortOrder,
            [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo, [FromQuery] int page = 0, [FromQuery] int size = 20)
        {
            var result = await _repository.GetAllBillAsync(GetCurrentUID(), name, sortOrder, page, size, dateFrom, dateTo);
            return Ok(result);
        }

        private string GetCurrentUID()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
