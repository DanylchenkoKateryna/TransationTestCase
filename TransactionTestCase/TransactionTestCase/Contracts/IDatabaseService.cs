using TransactionTestCase.Entity;

namespace TransactionTestCase.Contracts
{
    public interface IDatabaseService
    {
        Task SeedData(IEnumerable<Transaction> transactions);
        Task<IEnumerable<Transaction>> GetTransactionsByYearAsync(DateTime startDate, DateTime endDate);
    }
}
