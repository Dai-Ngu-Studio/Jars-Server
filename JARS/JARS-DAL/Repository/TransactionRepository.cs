using JARS_DAL.DAO;
using JARS_DAL.Models;
namespace JARS_DAL.Repository;

public class TransactionRepository : ITransactionRepository
{
    public Task<IEnumerable<Transaction>> GetTransactionsFromDate(DateTime date) => TransactionManagement.Instance.GetTransactionsFromDate(date);
    public Task<IEnumerable<Transaction>> GetTransactions(string uid) => TransactionManagement.Instance.GetTransactions(uid);
    public Task<Transaction?> GetTransaction(int id, string uid) => TransactionManagement.Instance.GetTransaction(id, uid);
    public Task Delete(Transaction transaction, string uid) => TransactionManagement.Instance.Delete(transaction, uid);
    public Task Update(Transaction transaction) => TransactionManagement.Instance.Update(transaction);
    public Task Add(Transaction transaction) => TransactionManagement.Instance.Add(transaction);
}