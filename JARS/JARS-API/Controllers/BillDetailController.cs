using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JARS_API.Controllers
{
    [ApiController]
    [Route("api/v1/bill-details")]
    [Authorize]
    public class BillDetailController : ControllerBase
    {
        private readonly IBillDetailRepository _repository;
        private readonly IBillRepository _billRepository;

        public BillDetailController(IBillDetailRepository repository, IBillRepository billRepository)
        {
            _repository = repository;
            _billRepository = billRepository;
        }

        /// <summary>
        /// Get all bill detail for a bill with bill_id. Only the owner of the account or admin is authorized to use this method.
        /// </summary>
        /// <param name="bill_id">bill_id of bill</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<BillDetail>>> GetBillDetailsWithBillId([FromQuery]int bill_id)
        {
            var result = await _repository.GetAllBillDetailWithBillIdAsync(bill_id);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BillDetail>> GetBillDetail(int id)
        {
            var billDetail = await _repository.GetBillDetailAsync(id);
            if (billDetail == null)
            {
                return NotFound();
            }

            return Ok(billDetail);
        }

        /// <summary>
        /// Update bill detail with id. Only the owner of the account or admin is authorized to use this method.
        /// </summary>
        /// <param name="id">id of bill detail</param>
        /// <param name="billDetail">BillDetail in JSON format</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBillDetail(int id, BillDetail billDetail)
        {
            var result = await _repository.GetBillDetailAsync(id);
            if (result.Id != billDetail.Id)
            {
                return BadRequest();
            }
            try
            {
                BillDetail _billDetail = new BillDetail
                {
                    Id = result.Id,
                    ItemName = billDetail.ItemName == null ? result.ItemName : billDetail.ItemName,
                    Price = billDetail.Price == null ? result.Price : billDetail.Price,
                    Quantity = billDetail.Quantity == null ? result.Quantity : billDetail.Quantity,
                    BillId = result.BillId
                };
                await _repository.UpdateBillDetailAsync(_billDetail);

                decimal? amount = 0;              
                if (result != null)
                {                   
                    var getAllCreatedBillDetails = await _repository.GetAllBillDetailWithBillIdAsync(_billDetail.BillId);
                    Bill bill = await _billRepository.GetBillByBillIdAsync(_billDetail.BillId, GetCurrentUID());

                    if (getAllCreatedBillDetails != null && bill != null)
                    {
                        foreach (var item in getAllCreatedBillDetails)
                        {
                            amount += item.Price * item.Quantity;
                        }
                        Bill _bill = new Bill
                        {
                            Id = bill.Id,
                            Name = bill.Name,
                            Date = bill.Date,
                            Amount = amount,
                        };
                        await _billRepository.UpdateBillAsync(_bill);
                        return CreatedAtAction("GetBillDetail", new { id = billDetail.Id }, _billDetail);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_repository.GetBillDetailAsync(billDetail.Id) == null)
                {
                    return NotFound();
                }
                else { throw; }
            }
            return Ok(billDetail);
        }

        /// <summary>
        /// Create bill detail for a bill with bill_id. Only the owner of the account or admin is authorized to use this method.
        /// </summary>
        /// <param name="bill_id">bill_id of bill</param>
        /// <param name="billDetail">BillDetail in JSON format</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateBillDetail([FromQuery]int bill_id, BillDetail billDetail)
        {
            Bill bill = await _billRepository.GetBillByBillIdAsync(bill_id, GetCurrentUID());

            if (bill == null)
                return NotFound();
            else
            {
                billDetail = new BillDetail
                {
                    ItemName = billDetail.ItemName,
                    Price = billDetail.Price,
                    Quantity = billDetail.Quantity,
                    BillId = bill.Id,
                };

                await _repository.CreateBillDetailAsync(billDetail);

                decimal? amount = 0;
                var getAllCreatedBillDetails = await _repository.GetAllBillDetailWithBillIdAsync(billDetail.BillId);

                if (getAllCreatedBillDetails != null)
                {
                    foreach (var item in getAllCreatedBillDetails)
                    {
                        amount += item.Price * item.Quantity;
                    }
                    Bill _bill = new Bill
                    {
                        Id = bill.Id,
                        Name = bill.Name,
                        Date = bill.Date,
                        Amount = amount,
                    };
                    await _billRepository.UpdateBillAsync(_bill);
                    return CreatedAtAction("GetBillDetail", new { id = billDetail.Id }, billDetail);
                } else
                {
                    return NotFound();
                }            
            }          
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBillDetail(int id)
        {
            BillDetail billDetail = new BillDetail
            {
                Id = id
            };
            try
            {
                await _repository.DeleteBillDetailAsync(billDetail);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(billDetail);
        }

        private string GetCurrentUID()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
