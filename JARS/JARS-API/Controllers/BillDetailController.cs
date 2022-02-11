using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JARS_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillDetailController : ControllerBase
    {
        private readonly IBillDetailRepository _repository;
        private readonly IBillRepository _billRepository;

        public BillDetailController(IBillDetailRepository repository, IBillRepository billRepository)
        {
            _repository = repository;
            _billRepository = billRepository;
        }

        [HttpGet("WithBillId/{id}")]
        public async Task<ActionResult<List<BillDetail>>> GetBillDetailsWithBillId(int id)
        {
            var result = await _repository.GetAllBillDetailWithBillIdAsync(id);
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

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBillDetail(int id, BillDetail billDetail)
        {
            if (id != billDetail.Id)
            {
                return BadRequest();
            }
            try
            {
                await _repository.UpdateBillDetailAsync(billDetail);

                decimal? amount = 0;
                var _billDetail = await _repository.GetBillDetailAsync(billDetail.Id);
                if (_billDetail != null)
                {                   
                    var getAllCreatedBillDetails = await _repository.GetAllBillDetailWithBillIdAsync(_billDetail.BillId);
                    Bill bill = await _billRepository.GetBillByBillIdAsync((int)_billDetail.BillId);

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
                        return CreatedAtAction("GetBillDetail", new { id = billDetail.Id }, billDetail);
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
        [HttpPost("{id}")]
        public async Task<ActionResult> CreateBillDetail(int id, BillDetail billDetail)
        {
            Bill bill = await _billRepository.GetBillByBillIdAsync(id);

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
        public async Task<IActionResult> DeleteTransaction(int id)
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
    }
}
