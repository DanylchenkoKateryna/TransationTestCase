using TransactionTestCase.Entity;

namespace TransactionTestCase.Contracts
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetTransactionsByUserTimeZoneAsync(IEnumerable<Transaction> transactions, double timeZoneCurrentUser);
        Task<IEnumerable<Transaction>> GetTransactionByClientTimeZoneAsync(IEnumerable<Transaction> transactions, DateTime startDate, DateTime endDate);
    }
}
