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
        private readonly IWalletReposiotry _walletReposiotry;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICategoryRepository _categoryRepository;

        public BillController(IBillRepository repository, IBillDetailRepository billDetailRepository, 
            IWalletReposiotry walletReposiotry, ICategoryRepository categoryRepository, ITransactionRepository transactionRepository)
        {
            _repository = repository;
            _billDetailRepository = billDetailRepository;
            _walletReposiotry = walletReposiotry;
            _categoryRepository = categoryRepository;
            _transactionRepository = transactionRepository;
        }

        //[HttpPost("?category_id={categoryId}")]
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

                var searchBillDetails = await _billDetailRepository.GetAllBillDetailWithBillIdAsync(_bill.Id, GetCurrentUID());
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
                await _repository.CreateBillAsync(bill);
                return CreatedAtAction("GetBill", new { id = bill.Id }, bill);
            }
        }           

        [HttpPut]
        public async Task<ActionResult> UpdateBill([FromQuery]int bill_id, [FromQuery]int wallet_id, Bill bill)
        {
            var result = await _repository.GetBillByBillIdAsync(bill_id, GetCurrentUID());
            if (result == null)
            {
                return BadRequest();
            }
            try
            {
                decimal? leftAmount = 0;
                if (bill.LeftAmount != null)
                {
                    leftAmount = result.LeftAmount - bill.LeftAmount;
                    if (leftAmount < 0)
                        leftAmount = 0;
                } else
                {
                    leftAmount = result.LeftAmount;
                }

                Bill _bill = new Bill
                {
                    Id = result.Id,
                    Date = bill.Date == null ? result.Date : bill.Date,
                    LeftAmount = leftAmount,
                    Name = bill.Name == null ? result.Name : bill.Name,
                    Amount = result.Amount,
                    CategoryId = result.CategoryId,
                    ContractId = result.ContractId,
                };
                await _repository.UpdateBillAsync(_bill);
                
                if (result.LeftAmount != leftAmount)
                {
                    var walletOwner = await _walletReposiotry.GetWallet(wallet_id);

                    if (walletOwner == null)
                        return NotFound(); 
                    Transaction transaction = new Transaction
                    {
                        WalletId = walletOwner.Id,
                        TransactionDate = DateTime.Now,
                        BillId = _bill.Id,
                        Amount = bill.LeftAmount,
                    };
                    await _transactionRepository.Add(transaction);
                }
            }
            catch (DbUpdateConcurrencyException)
            {             
                if (_repository.GetBillByBillIdAsync(bill.Id, GetCurrentUID()) == null)
                {
                    return NotFound();
                }
                else { throw; }
            }
            return Ok(bill);
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
                var billDetail = await _billDetailRepository.GetAllBillDetailWithBillIdAsync(bill.Id, GetCurrentUID());
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

        [HttpGet]
        public async Task<ActionResult<List<Bill>>> GetAllBillsForContract([FromQuery] int contract_id)
        {
            var result = await _repository.GetAllBillByContractIdAsync(contract_id, GetCurrentUID());
            return Ok(result);
        }

        private string GetCurrentUID()
        {
            ClaimsPrincipal httpUser = HttpContext.User as ClaimsPrincipal;
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
