using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JARS_API.Controllers
{
    [Route("api/v1/schedule-types")]
    [ApiController]
    public class ScheduleTypeController : ControllerBase
    {
        private readonly IScheduleTypeRepository _scheduleTypeRepository;
        private readonly IAccountRepository _accountRepository;

        public ScheduleTypeController(IScheduleTypeRepository scheduleTypeRepository, IAccountRepository accountRepository)
        {
            _scheduleTypeRepository = scheduleTypeRepository;
            _accountRepository = accountRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<ScheduleType>>> GetList()
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                var scheduleTypes = await _scheduleTypeRepository.GetListAsync();
                if (scheduleTypes != null)
                {
                    return scheduleTypes.ToList();
                }
                return NoContent();
            }
            return Unauthorized();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ScheduleType>> GetScheduleType(int id)
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                var scheduleType = await _scheduleTypeRepository.GetAsync(id);
                if (scheduleType == null)
                {
                    return NotFound();
                }
                return scheduleType;
            }
            return Unauthorized();
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutScheduleType(int id, ScheduleType scheduleType)
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                var user = await _accountRepository.GetAsync(uid);
                if (user != null && user.IsAdmin)
                {
                    if (id != scheduleType.Id)
                    {
                        return BadRequest();
                    }
                    try
                    {
                        await _scheduleTypeRepository.UpdateAsync(scheduleType);
                        return Ok(scheduleType);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        // to-do logging
                        if (!ScheduleTypeExists(scheduleType.Id))
                        {
                            return NotFound();
                        }
                        return StatusCode(500);
                    }
                    catch (DbUpdateException)
                    {
                        // to-do logging
                        if (!ScheduleTypeExists(scheduleType.Id))
                        {
                            return NotFound();
                        }
                        return StatusCode(500);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex);
                    }
                }
            }
            return Unauthorized();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ScheduleType>> PostScheduleType(ScheduleType scheduleType)
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                var user = await _accountRepository.GetAsync(uid);
                if (user != null && user.IsAdmin)
                {
                    try
                    {
                        await _scheduleTypeRepository.AddAsync(scheduleType);
                        return CreatedAtAction(nameof(GetScheduleType), new { id = scheduleType.Id }, scheduleType);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        // to-do logging
                        if (ScheduleTypeExists(scheduleType.Id))
                        {
                            return Conflict();
                        }
                        return StatusCode(500);
                    }
                    catch (DbUpdateException)
                    {
                        // to-do logging
                        if (ScheduleTypeExists(scheduleType.Id))
                        {
                            return Conflict();
                        }
                        return StatusCode(500);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex);
                    }
                }
            }
            return Unauthorized();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteScheduleType(int id)
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                var user = await _accountRepository.GetAsync(uid);
                if (user != null && user.IsAdmin)
                {
                    ScheduleType? scheduleType = await _scheduleTypeRepository.GetAsync(id);
                    if (scheduleType == null)
                    {
                        return BadRequest();
                    }
                    try
                    {
                        await _scheduleTypeRepository.DeleteAsync(scheduleType);
                        return Ok();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        // to-do logging
                        if (!ScheduleTypeExists(scheduleType.Id))
                        {
                            return NotFound();
                        }
                        return StatusCode(500);
                    }
                    catch (DbUpdateException)
                    {
                        // to-do logging
                        if (!ScheduleTypeExists(scheduleType.Id))
                        {
                            return NotFound();
                        }
                        return StatusCode(500);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex);
                    }
                }
            }
            return Unauthorized();
        }

        private bool ScheduleTypeExists(int id)
        {
            return _scheduleTypeRepository.GetAsync(id) != null;
        }
    }
}
