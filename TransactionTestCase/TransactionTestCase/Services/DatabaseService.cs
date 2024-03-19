using Dapper;
using Microsoft.Data.SqlClient;
using TransactionTestCase.Contracts;
using TransactionTestCase.Entity;

namespace TransactionTestCase.Services
{
    public class DatabaseService:IDatabaseService
    {
        private readonly IConfiguration _configuration;

        public DatabaseService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public async Task SeedData(IEnumerable<Transaction> records)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();
                foreach (var record in records)
                {
                    var existingRecord = await connection.QueryFirstOrDefaultAsync<Transaction>(
                        "SELECT * FROM Transactions WHERE TransactionId = @TransactionId", new { TransactionId = record.TransactionId });

                    if (existingRecord == null)
                    {
                        await connection.ExecuteAsync(
                            @"INSERT INTO Transactions (TransactionId, Name, Email, Amount, TransactionDate, ClientLocation) 
                              VALUES (@TransactionId, @Name, @Email, @Amount, @TransactionDate, @ClientLocation)",
                            record);
                    }
                    else
                    {
                        await connection.ExecuteAsync(
                            @"UPDATE Transactions 
                              SET Name = @Name, Email = @Email, Amount = @Amount, TransactionDate = @TransactionDate, ClientLocation = @ClientLocation 
                              WHERE TransactionId = @TransactionId",
                            record);
                    }
                }
            }
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByYearAsync(DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                await connection.OpenAsync();
                string query = @"
                        SELECT *
                        FROM Transactions
                        WHERE TransactionDate >= @StartDate
                        AND TransactionDate <= @EndDate";

                return await connection.QueryAsync<Transaction>(query, new { StartDate = startDate, EndDate = endDate });
            }
        }
    }
}
