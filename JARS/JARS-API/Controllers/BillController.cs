using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JARS_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BillController : ControllerBase
    {
        private readonly IBillRepository _repository;
        private readonly IBillDetailRepository _billDetailRepository;

        public BillController(IBillRepository repository, IBillDetailRepository billDetailRepository)
        {
            _repository = repository;
            _billDetailRepository = billDetailRepository;
        }

        [HttpPost]
        public async Task<ActionResult> CreateBill(Bill bill)
        {
            if (bill.BillDetails.Count > 0)
            {
                Bill _bill = new Bill
                {
                    Id = bill.Id,
                    Date = bill.Date,
                    Name = bill.Name,
                    LeftAmount = bill.LeftAmount,
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
                var searchBill = await _repository.GetBillByBillIdAsync(_bill.Id);
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
                        LeftAmount = searchBill.LeftAmount,
                        Amount = amount,
                    };
                    await _repository.UpdateBillAsync(_bill);
                } else
                {
                    return NotFound();
                }

                return CreatedAtAction("GetBill", new { id = bill.Id }, bill);
            } else
            {
                await _repository.CreateBillAsync(bill);
                return CreatedAtAction("GetBill", new { id = bill.Id }, bill);
            }           
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBill(int id, Bill bill)
        {
            if (id != bill.Id)
            {
                return BadRequest();
            }
            try
            {
                await _repository.UpdateBillAsync(bill);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_repository.GetBillByBillIdAsync(bill.Id) == null)
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
            return await _repository.GetBillByBillIdAsync(id);
        }
        [HttpGet]
        public async Task<ActionResult<List<Bill>>> GetAllBill()
        {
            var result = await _repository.GetBillsAsync();
            return Ok(result);
        }
    }
}
