using FirebaseAdmin;
using FirebaseAdmin.Auth;
using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace JARS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountRepository _accountRepository;
        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// Get account with UID.
        /// </summary>
        /// <param name="authorization">Format: Bearer <token></param>
        /// <param name="id">UID of account</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Account>> GetAccount(
            [FromHeader(Name = "Authorization")] string authorization,
            string id)
        {
            var account = await _accountRepository.GetAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return account;
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutAccount([FromHeader(Name = "Authorization")] string authorization, string id, Account account)
        {
            if (!id.Equals(account.Id))
            {
                return BadRequest();
            }
            try
            {
                await _accountRepository.UpdateAsync(account);
            }
            catch (DbUpdateConcurrencyException)
            {
                // to-do logging
                return StatusCode(500);
            }
            catch (DbUpdateException)
            {
                // to-do logging
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return Ok(account);
        }

        /// <summary>
        /// This method has not been implemented yet.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount([FromHeader(Name = "Authorization")] string authorization, string id)
        {
            return NoContent();
        }

        /// <summary>
        /// This method is used for signing in/ creating an account.
        /// </summary>
        /// <param name="authorization">Format: Bearer <token></param>
        /// <returns></returns>
        [HttpPost("login")]
        [Authorize]
        public async Task<IActionResult> Login([FromHeader(Name = "Authorization")] string authorization)
        {
            ClaimsPrincipal httpUser = HttpContext.User as ClaimsPrincipal;
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                Console.WriteLine($"api/Account/login: Authorized user with UID: {uid}");
                try
                {
                    Account? account = await _accountRepository.GetAsync(uid);
                    if (account != null)
                    {
                        Console.WriteLine($"api/Account/login: User {uid} had already created an account.");
                        return Ok(account);
                    }
                    else
                    {
                        try
                        {
                            return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, account);
                            Console.WriteLine($"api/Account/login: Creating account for user {uid} ...");
                            UserRecord? userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
                            bool isUserRecordExisted = userRecord != null;
                            string? displayName = isUserRecordExisted ? userRecord?.DisplayName : uid;
                            string? email = userRecord?.Email;
                            string? photoUrl = userRecord?.PhotoUrl;
                            DateTime? tokenCreatedTime = isUserRecordExisted ? userRecord?.TokensValidAfterTimestamp : DateTime.Now;
                            Account newAccount = new Account
                            {
                                Id = uid,
                                DisplayName = displayName,
                                Email = email,
                                PhotoUrl = photoUrl,
                                LastLoginDate = tokenCreatedTime,
                            };
                            await _accountRepository.AddAsync(newAccount);
                            return Ok(newAccount);
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            // to-do logging
                            return StatusCode(500);
                        }
                        catch (DbUpdateException)
                        {
                            // to-do logging
                            return StatusCode(500);
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex);
                        }
                    }
                }
                catch (Exception)
                {
                    // to-do logging
                    return StatusCode(500);
                }
            }
            return BadRequest();
        }

        /// <summary>
        /// This method is used to verify if a token is valid. It should only be used in development.
        /// </summary>
        /// <param name="json">The body of the request should have Content-Type 'application/json', the key "token" with the token as the value.</param>
        /// <returns>
        /// <para>201 ACCEPTED if token is valid</para>
        /// <para>401 BAD REQUEST if token is invalid</para>
        /// </returns>
        [HttpPost("verify-token")]
        public async Task<IActionResult> VerifyToken([FromBody] JsonElement json)
        {
            var auth = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance;
            string? token = json.GetProperty("token").GetString();
            try
            {
                var response = await auth.VerifyIdTokenAsync(token);
                if (response != null)
                {
                    string uid = ((FirebaseToken)response).Uid;
                    return Accepted();
                }
            }
            catch (FirebaseAuthException)
            {
                return BadRequest("Invalid token. The token might have expired.");
            }
            catch (Exception)
            {
                return BadRequest();
            }
            return BadRequest();
        }
    }
}
