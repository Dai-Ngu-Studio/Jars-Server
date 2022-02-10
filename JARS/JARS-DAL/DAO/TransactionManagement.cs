using JARS_DAL.Models;
using Microsoft.EntityFrameworkCore;
namespace JARS_DAL.DAO;

public class TransactionManagement
{
    private static TransactionManagement instance = null;
    private static readonly object instanceLock = new object();

    private TransactionManagement() { }
    public static TransactionManagement Instance
    {
        get
        {
            lock (instanceLock)
            {
                if (instance == null)
                {
                    instance = new TransactionManagement();
                }
                return instance;
            }
        }
    }
    public async Task<IEnumerable<Transaction>> GetTransactions()
    {
        try
        {
            var context = new JarsDatabaseContext();
            return await context.Transactions.ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<Transaction?> GetTransaction(int id)
    {
        try
        {
            var context = new JarsDatabaseContext();
            return await context.Transactions.FindAsync(id);
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task Add(Transaction transaction)
    {
        try
        {
            var context = new JarsDatabaseContext();
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task Update(Transaction transaction)
    {
        try
        {
            var context = new JarsDatabaseContext();
            context.Update(transaction);
            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task Delete(Transaction transaction)
    {
        try
        {
            Transaction? _transaction = await GetTransaction(transaction.Id);
            var context = new JarsDatabaseContext();
            context.Transactions.Remove(transaction);
            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
}