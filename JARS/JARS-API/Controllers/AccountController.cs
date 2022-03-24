using FirebaseAdmin.Auth;
using JARS_API.BusinessModels;
using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JARS_API.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountDeviceRepository _accountDeviceRepository;

        public AccountController(IAccountRepository accountRepository, IAccountDeviceRepository accountDeviceRepository)
        {
            _accountRepository = accountRepository;
            _accountDeviceRepository = accountDeviceRepository;
        }

        /// <summary>
        /// Get accounts with optional queries. Only the admin is authorized to use this method.
        /// </summary>
        /// <param name="page">Parameter "page" is multiplied by the parameter "size" to determine the number of rows to skip. Default value: 0</param>
        /// <param name="size">Maximum number of results to return. Default value: 10</param>
        /// <param name="search">Optional filter. Default value: null</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<AccountWithTransactionCount>>> GetList(
            [FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? search = null!)
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                var user = await _accountRepository.GetAsync(uid);
                if (user != null && user.IsAdmin)
                {
                    var accounts = await _accountRepository.GetListAsync(page, size, search!);
                    if (accounts != null)
                    {
                        List<AccountWithTransactionCount> accountWithTransactions = new List<AccountWithTransactionCount>();
                        foreach (var account in accounts)
                        {
                            int transactionCount = 0;
                            foreach (var wallet in account.Wallets)
                            {
                                transactionCount += wallet.Transactions.Count();
                            }
                            AccountWithTransactionCount accountWithTransaction = new()
                            {
                                IsAdmin = account.IsAdmin,
                                DisplayName = account.DisplayName,
                                Email = account.Email,
                                Id = account.Id,
                                LastLoginDate = account.LastLoginDate,
                                PhotoUrl = account.PhotoUrl,
                                TransactionCount = transactionCount,
                            };
                            accountWithTransactions.Add(accountWithTransaction);
                        }
                        int TotalAccounts = await _accountRepository.GetTotalAccount(search!);
                        int TotalPages = (TotalAccounts - 1) / size + 1;
                        var model = new
                        {
                            accounts = accountWithTransactions,
                            numOfPages = TotalPages,
                        };
                        return Ok(model);
                    }
                    return NoContent();
                }
            }
            return Unauthorized();
        }

        /// <summary>
        /// Get account with UID. Only the owner of the account or admin is authorized to use this method.
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
        /// Update account with UID. Only the owner of the account or admin is authorized to use this method.
        /// Only admin is allowed to change the role of an account. Admin can't change their own role.
        /// "isAdmin" should be false if no role change is desired.
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
                        account.LastLoginDate = existedAccount.LastLoginDate;
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
        /// Erase all data of account with UID. Only the owner of the account or admin is authorized to use this method.
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
                var user = await _accountRepository.GetAsync(uid);
                if (uid.Equals(id) || (user != null && user.IsAdmin))
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
        /// <param name="FcmToken">Device registration token</param>
        /// <returns></returns>
        [HttpPost("login")]
        [Authorize]
        public async Task<ActionResult> Login([FromHeader] string? FcmToken)
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                Console.WriteLine($"Accounts: Authorized user with UID: {uid}");
                Account? account = await _accountRepository.GetAsync(uid);
                if (account != null)
                {
                    try
                    {
                        Console.WriteLine($"Accounts: User {uid} had already created an account.");
                        UserRecord? userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
                        bool isUserRecordExisted = userRecord != null;
                        DateTime? tokenCreatedTime = DateTime.Now;
                        account.LastLoginDate = tokenCreatedTime;
                        await _accountRepository.UpdateAsync(account);
                        if (FcmToken != null)
                        {
                            AccountDevice? accountDevice = await _accountDeviceRepository.GetAsync(FcmToken);
                            if (accountDevice != null)
                            {
                                accountDevice.LastActiveDate = DateTime.Now;
                                await _accountDeviceRepository.UpdateAsync(accountDevice);
                            }
                            else
                            {
                                Console.WriteLine($"Accounts: Registering new device of user {uid}");
                                AccountDevice newAccountDevice = new AccountDevice
                                {
                                    FcmToken = FcmToken,
                                    AccountId = account.Id,
                                    LastActiveDate = DateTime.Now,
                                };
                                await _accountDeviceRepository.AddAsync(newAccountDevice);
                            }
                        }
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
                        return BadRequest(ex.StackTrace);
                    }
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
                        DateTime? tokenCreatedTime = DateTime.Now;
                        Account newAccount = new Account
                        {
                            Id = uid,
                            DisplayName = displayName,
                            IsAdmin = false,
                            Email = email,
                            PhotoUrl = photoUrl,
                            LastLoginDate = tokenCreatedTime,
                        };
                        await _accountRepository.AddAsync(newAccount);
                        if (FcmToken != null)
                        {
                            Console.WriteLine($"Accounts: Registering new device of user {uid}");
                            AccountDevice newAccountDevice = new AccountDevice
                            {
                                FcmToken = FcmToken,
                                AccountId = newAccount.Id,
                                LastActiveDate = DateTime.Now,
                            };
                            await _accountDeviceRepository.AddAsync(newAccountDevice);
                        }
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
                        return BadRequest(ex.StackTrace);
                    }
                }
            }
            return Unauthorized();
        }

        private bool AccountExists(string id)
        {
            return _accountRepository.GetAsync(id) != null;
        }
    }
}
