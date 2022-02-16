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
    public class BillController : ControllerBase
    {
        private readonly IBillRepository _repository;
        private readonly IBillDetailRepository _billDetailRepository;
        private readonly IContractRepository _contractRepository;
        private readonly ICategoryRepository _categoryRepository;

        public BillController(IBillRepository repository, IBillDetailRepository billDetailRepository, 
            IContractRepository contractRepository, ICategoryRepository categoryRepository)
        {
            _repository = repository;
            _billDetailRepository = billDetailRepository;
            _contractRepository = contractRepository;
            _categoryRepository = categoryRepository;
        }

        //[HttpPost("?contract_id={contractId}")]
        [HttpPost]
        public async Task<ActionResult> CreateBillForContract([FromQuery]int contract_id,
             [FromQuery] int category_id, Bill bill)
        {
            var contract = await _contractRepository.GetContractByContractIdAsync(contract_id, GetCurrentUID());
            var category = await _categoryRepository.GetCategoryByCategoryIdAsync(category_id);

            if (contract != null)
            {
                if (bill.BillDetails.Count > 0)
                {
                    Bill _bill = new Bill
                    {
                        Date = bill.Date,
                        Name = bill.Name,
                        CategoryId = category != null ? category.Id : null,
                        ContractId = contract.Id,
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
                            ContractId = contract.Id,
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
            return NotFound();
        }           

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBill(int id, Bill bill)
        {
            var result = await _repository.GetBillByBillIdAsync(id, GetCurrentUID());
            if (result == null)
            {
                return BadRequest();
            }
            try
            {
                decimal? leftAmount = 0;
                if (bill.LeftAmount != null)
                {
                    leftAmount = bill.LeftAmount - result.LeftAmount;
                    if (leftAmount < 0)
                        return BadRequest("left amount below zero");
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

        [HttpGet("WithContractId/{contractId}")]
        public async Task<ActionResult<List<Bill>>> GetAllBillsForContract(int contractId)
        {
            var result = await _repository.GetBillByContractIdAsync(contractId, GetCurrentUID());
            return Ok(result);
        }

        private string GetCurrentUID()
        {
            ClaimsPrincipal httpUser = HttpContext.User as ClaimsPrincipal;
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
