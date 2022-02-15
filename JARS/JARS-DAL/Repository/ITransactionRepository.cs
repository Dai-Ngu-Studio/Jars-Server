using JARS_DAL.Models;
namespace JARS_DAL.Repository;


public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetTransactions(string uid);
    Task<Transaction?> GetTransaction(int id,string uid);
    Task Delete(Transaction transaction,string uid);
    Task Update(Transaction transaction);
    Task Add(Transaction transaction);
}