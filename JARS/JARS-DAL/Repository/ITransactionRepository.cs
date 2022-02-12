using JARS_DAL.Models;
namespace JARS_DAL.Repository;


public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetTransactions();
    Task<Transaction?> GetTransaction(int id);
    Task Delete(Transaction transaction);
    Task Update(Transaction transaction);
    Task Add(Transaction transaction);
}