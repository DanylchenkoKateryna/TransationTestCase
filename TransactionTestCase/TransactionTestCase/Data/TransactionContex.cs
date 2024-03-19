using Microsoft.EntityFrameworkCore;
using TransactionTestCase.Entity;

namespace TransactionTestCase.Data
{
    public class TransactionContex : DbContext
    {
        public TransactionContex()
        {

        }
        public TransactionContex(DbContextOptions<TransactionContex> options) : base(options)
        {
        }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
