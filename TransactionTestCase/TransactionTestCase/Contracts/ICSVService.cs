using TransactionTestCase.Entity;

namespace TransactionTestCase.Contracts
{
    public interface ICSVService
    {
        public IEnumerable<Transaction> ReadCSV(Stream file);
        void WriteToCSV(IEnumerable<Transaction> transactions, string fileName);
    }
}
