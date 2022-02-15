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
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }



        /// <summary>
        /// Get accounts with optional queries. Only the admin is authorized to use this method. (Note: Admin check is currently not implemented yet).
        /// </summary>
        /// <param name="authorization">Format: Bearer (token)</param>
        /// <param name="page">Parameter "page" is multiplied by the parameter "size" to determine the number of rows to skip. Default value: 0</param>
        /// <param name="size">Maximum number of results to return. Default value: 20</param>
        /// <param name="email">Optional filter for account's email. Default value: ""</param>
        /// <param name="displayName">Optional filter for account's displayName. Default value: ""</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Account>>> GetAll([FromHeader] string authorization,
            [FromQuery] int page = 0, [FromQuery] int size = 20, [FromQuery] string? email = "", [FromQuery] string? displayName = "")
        {
            ClaimsPrincipal httpUser = HttpContext.User as ClaimsPrincipal;
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                // to-do check if admin
                var accounts = await _accountRepository.GetListAsync(page, size, email, displayName);
                if (accounts != null && accounts.Count() > 0)
                {
                    return accounts.ToList();
                }
                return NotFound();
            }
            return Unauthorized();
        }

        /// <summary>
        /// Get account with UID. Only the owner of the account/admin is authorized to use this method.
        /// </summary>
        /// <param name="authorization">Format: Bearer (token)</param>
        /// <param name="id">UID of account</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Account>> GetAccount(
            [FromHeader(Name = "Authorization")] string authorization,
            string id)
        {
            ClaimsPrincipal httpUser = HttpContext.User as ClaimsPrincipal;
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                // to-do check if admin
                if (uid.Equals(id))
                {
                    var account = await _accountRepository.GetAsync(id);
                    if (account == null)
                    {
                        return NotFound();
                    }
                    return account;
                }
            }
            return Unauthorized();
        }

        /// <summary>
        /// Update account with UID. Only the owner of the account is authorized to use this method.
        /// </summary>
        /// <param name="authorization">Format: Bearer (token)</param>
        /// <param name="id">UID of account</param>
        /// <param name="account">Account in JSON format</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutAccount([FromHeader(Name = "Authorization")] string authorization, string id, Account account)
        {
            ClaimsPrincipal httpUser = HttpContext.User as ClaimsPrincipal;
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                if (id.Equals(account.Id))
                {
                    try
                    {
                        await _accountRepository.UpdateAsync(account);
                        return Ok(account);
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
            return Unauthorized();
        }

        /// <summary>
        /// Erase all data of an account. Only the owner of the account is authorized to use this method.
        /// </summary>
        /// <param name="authorization">Format: Bearer (token)</param>
        /// <param name="id">UID of account</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount([FromHeader(Name = "Authorization")] string authorization, string id)
        {
            ClaimsPrincipal httpUser = HttpContext.User as ClaimsPrincipal;
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                if (uid.Equals(id))
                {
                    var auth = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance;
                    Account? account = await _accountRepository.GetIncludedAsync(id);
                    if (account != null)
                    {
                        try
                        {
                            await _accountRepository.DeleteAsync(account);
                            await auth.DeleteUserAsync(id);
                            return Ok();
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
            }
            return Unauthorized();
        }

        /// <summary>
        /// This method is used for signing in/creating an account.
        /// </summary>
        /// <param name="authorization">Format: Bearer (token)</param>
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
            return Unauthorized();
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
