﻿using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using JARS_API.BusinessModels;

namespace JARS_API.Controllers
{
    [Route("api/v1/wallets")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletReposiotry repository;
        public WalletController(IWalletReposiotry repository)
        {
            this.repository = repository;
        }
        [HttpGet]
        [Authorize]
        /// <summary>
        /// This method will return list of wallets upon accountId
        /// </summary>
        public async Task<IEnumerable<Wallet>> GetWallets()
        {
            ClaimsPrincipal httpUser = HttpContext.User as ClaimsPrincipal;           
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                if (await repository.GetAllWallets(uid) != null)
                {
                    return await repository.GetAllWallets(uid);
                }
            }
            return null;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Wallet>> GetWallet(int id)
        {
            return await repository.GetWallet(id);
        }
        [HttpGet("wallet-spend/{id}")]
        public async Task<ActionResult<TransactionWallet>> GetWalletSpend(int id)
        {
            ClaimsPrincipal httpUser = HttpContext.User as ClaimsPrincipal;
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                if (await repository.getWalletTransaction(uid, id) == null)
                {
                    return BadRequest("The wallet may not exitst");
                }
            }
            return await repository.getWalletTransaction(uid,id);
        }
        [HttpGet("six-wallets-spend")]
        public async Task<ActionResult<List<TransactionWallet>>> GetSixWalletSpend()
        {
            ClaimsPrincipal httpUser = HttpContext.User as ClaimsPrincipal;
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                if (await repository.getTransactionWallets(uid) == null)
                {
                    return BadRequest("user may not init 6 jars");
                }
            }
            return await repository.getTransactionWallets(uid);
        }
        //POST /wallets/
        [HttpPost]
        public async Task AddWallet(Wallet wallet)
        {
            Wallet _wallet = new Wallet
            {
                AccountId = wallet.AccountId,
                Name = wallet.Name,
                StartDate = wallet.StartDate,
                WalletAmount = wallet.WalletAmount,
                Percentage = wallet.Percentage,
            };
                  await repository.AddWallet(_wallet);
        }
       

        //PUT /wallets/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateWallet(int id,Wallet wallet)
        {

            if (id != wallet.Id)
            {
                return BadRequest();
            }
            try
            {
                await repository.UpdateWallet(wallet);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (repository.GetWallet(wallet.Id) == null)
                {
                    return NotFound();
                }
                else { throw; }
            }
            return Ok(wallet);

        }
        //Delete /wallets/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWallet(int id)
        {
           repository.DeleteWallet(id);
            return Ok();

        }
        [HttpPost("six-wallets")]
        [Authorize]
        public async Task<ActionResult> CreateSixJars([FromQuery] decimal totalAmount)
        {
            ClaimsPrincipal httpUser = HttpContext.User as ClaimsPrincipal;
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                if (await repository.countWallets(uid) == 0)
                {
                    if (await repository.GetAllWallets(uid) != null)
                    {
                        await repository.Add6DefaultJars(uid, totalAmount);
                    }
                    return Ok("Add success 6 Jars");
                }
                else return BadRequest("This account already have more than 1 wallet, cannot create 6 default jars");

            }
            return BadRequest("Authorize problem or wrong input");


        }

    }
}
