#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;
namespace JARS_API.Controllers;

[Route("api/v1/files")]
[Authorize]
[ApiController]
public class FileController :ControllerBase
{
    private ITransactionRepository _transactionRep;
    public FileController(ITransactionRepository repository)
    {
        _transactionRep = repository;
    }
    // GET: api/Transaction
    [HttpGet("transactions")]
    public async Task<FileResult> GetFileTransactions()
    {
        var builder = new StringBuilder();
        builder.AppendLine("Amount,Wallet name,Date");

        var list = await _transactionRep.GetTransactions(GetCurrentUID());
        foreach (var transaction in list)
        {
            builder.AppendLine($"{transaction.Amount},{transaction.Wallet.Name},{transaction.TransactionDate}");
        }
        return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "Transaction_log.csv");

    }
    private string GetCurrentUID()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}