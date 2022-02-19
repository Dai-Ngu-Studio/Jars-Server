using FirebaseAdmin.Auth;
using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace JARS_API.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// Get accounts with optional queries. Only the admin is authorized to use this method.
        /// </summary>
        /// <param name="page">Parameter "page" is multiplied by the parameter "size" to determine the number of rows to skip. Default value: 0</param>
        /// <param name="size">Maximum number of results to return. Default value: 20</param>
        /// <param name="email">Optional filter for account's email. Default value: ""</param>
        /// <param name="displayName">Optional filter for account's displayName. Default value: ""</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Account>>> GetList(
            [FromQuery] int page = 0, [FromQuery] int size = 20, [FromQuery] string? email = "", [FromQuery] string? displayName = "")
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null && email != null && displayName != null)
            {
                var user = await _accountRepository.GetAsync(uid);
                if (user != null && user.IsAdmin)
                {
                    var accounts = await _accountRepository.GetListAsync(page, size, email, displayName);
                    if (accounts != null)
                    {
                        return accounts.ToList();
                    }
                    return NoContent();
                }
            }
            return Unauthorized();
        }

        /// <summary>
        /// Get account with UID. Only the owner of the account/admin is authorized to use this method.
        /// </summary>
        /// <param name="id">UID of account</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Account>> GetAccount(string id)
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                var user = await _accountRepository.GetAsync(uid);
                if (uid.Equals(id) || (user != null && user.IsAdmin))
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
        /// Update account with UID. Only the owner of the account/admin is authorized to use this method.
        /// Only admin is allowed to change the role of an account. Admin can't change their own role.
        /// </summary>
        /// <param name="id">UID of account</param>
        /// <param name="account">Account in JSON format</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutAccount(string id, Account account)
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                var user = await _accountRepository.GetAsync(uid);
                var existedAccount = await _accountRepository.GetAsync(id);
                if (uid.Equals(id) || (user != null && user.IsAdmin))
                {
                    try
                    {
                        if (existedAccount == null)
                        {
                            return BadRequest();
                        }
                        // check if role was changed
                        if (account.IsAdmin != existedAccount.IsAdmin)
                        {
                            if (user == null)
                            {
                                return Unauthorized();
                            }
                            // check if user is not admin
                            if (!user.IsAdmin)
                            {
                                return Unauthorized();
                            }
                            // check if user is changing role of self
                            if (account.Id.Equals(user.Id))
                            {
                                return BadRequest("User not permitted to change role of self.");
                            }
                        }
                        await _accountRepository.UpdateAsync(account);
                        return Ok(account);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        // to-do logging
                        if (!AccountExists(id))
                        {
                            return NotFound();
                        }
                        return StatusCode(500);
                    }
                    catch (DbUpdateException)
                    {
                        // to-do logging
                        if (!AccountExists(id))
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

        /// <summary>
        /// Erase all data of an account. Only the owner of the account is authorized to use this method.
        /// </summary>
        /// <param name="id">UID of account</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteAccount(string id)
        {
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
                            if (!AccountExists(id))
                            {
                                return NotFound();
                            }
                            return StatusCode(500);
                        }
                        catch (DbUpdateException)
                        {
                            // to-do logging
                            if (!AccountExists(id))
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
            }
            return Unauthorized();
        }

        /// <summary>
        /// This method is used for signing in/creating an account.
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        [Authorize]
        public async Task<ActionResult> Login()
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                Console.WriteLine($"Accounts: Authorized user with UID: {uid}");
                Account? account = await _accountRepository.GetAsync(uid);
                if (account != null)
                {
                    Console.WriteLine($"Accounts: User {uid} had already created an account.");
                    UserRecord? userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
                    bool isUserRecordExisted = userRecord != null;
                    DateTime? tokenCreatedTime = isUserRecordExisted ? userRecord?.TokensValidAfterTimestamp : DateTime.Now;
                    account.LastLoginDate = tokenCreatedTime;
                    await _accountRepository.UpdateAsync(account);
                    return Ok(account);
                }
                else
                {
                    try
                    {
                        Console.WriteLine($"Accounts: Creating account for user {uid} ...");
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

        private bool AccountExists(string id) { 
            return _accountRepository.GetAsync(id) != null;
        }

        ///// <summary>
        ///// This method is used to verify if a token is valid. It should only be used in development.
        ///// </summary>
        ///// <param name="json">The body of the request should have Content-Type 'application/json', the key "token" with the token as the value.</param>
        ///// <returns>
        ///// <para>200 OK if token is valid</para>
        ///// <para>401 BAD REQUEST if token is invalid</para>
        ///// </returns>
        //[HttpPost("verify-token")]
        //public async Task<ActionResult> VerifyToken([FromBody] JsonElement json)
        //{
        //    var auth = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance;
        //    string? token = json.GetProperty("token").GetString();
        //    try
        //    {
        //        var response = await auth.VerifyIdTokenAsync(token);
        //        if (response != null)
        //        {
        //            string uid = ((FirebaseToken)response).Uid;
        //            return Ok();
        //        }
        //    }
        //    catch (FirebaseAuthException)
        //    {
        //        return BadRequest("Invalid token. The token might have expired.");
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest();
        //    }
        //    return BadRequest();
        //}
    }
}
