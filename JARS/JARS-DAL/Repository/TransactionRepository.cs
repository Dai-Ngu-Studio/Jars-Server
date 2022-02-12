using JARS_DAL.DAO;
using JARS_DAL.Models;
namespace JARS_DAL.Repository;

public class TransactionRepository : ITransactionRepository
{
    public Task<IEnumerable<Transaction>> GetTransactions() => TransactionManagement.Instance.GetTransactions();
    public Task<Transaction?> GetTransaction(int id) => TransactionManagement.Instance.GetTransaction(id);
    public Task Delete(Transaction transaction) => TransactionManagement.Instance.Delete(transaction);
    public Task Update(Transaction transaction) => TransactionManagement.Instance.Update(transaction);
    public Task Add(Transaction transaction) => TransactionManagement.Instance.Add(transaction);
}