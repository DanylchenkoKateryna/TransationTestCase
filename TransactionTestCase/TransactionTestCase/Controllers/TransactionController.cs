using Microsoft.AspNetCore.Mvc;
using TransactionTestCase.Entity;
using TransactionTestCase.Contracts;
using Microsoft.Data.SqlClient;
using Dapper;
using TransactionTestCase.Models;

namespace TransactionTestCase.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ICSVService _csvService;
        private readonly IDatabaseService _databaseService;
        private readonly ITransactionService _transactionService;
        private readonly IConfiguration _configuration;
        public TransactionController(ILogger<TransactionController> logger,ICSVService cSVService,
            IDatabaseService databaseService, IConfiguration configuration, ITransactionService transactionService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _csvService = cSVService ?? throw new ArgumentNullException(nameof(cSVService));
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _transactionService = transactionService;
        }

        [HttpPost("UploadFile")]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogError("No file uploaded or file is empty.");
                return BadRequest("Please upload a valid file.");
            }

            _logger.LogInformation($"Processing file: {file.FileName}");

            try
            {
                var records = _csvService.ReadCSV(file.OpenReadStream());
                if (records == null || !records.Any())
                {
                    _logger.LogWarning("No valid records found in the uploaded file.");
                    return BadRequest("No valid records found in the uploaded file.");
                }

                await _databaseService.SeedData(records);

                _logger.LogInformation("Data inserted successfully.");
                return Ok("Data inserted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing file: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the file.");
            }
        }

     
        [HttpPost("ExportToCsv")]
        public async Task<ActionResult> ExportTransactionsToCsv(string fileName,[FromBody] ExportRequest request)
        {
            if (request == null || request.SelectedColumns == null || !request.SelectedColumns.Any())
            {
                return BadRequest("Invalid export request.");
            }
            try
            {
                string query = $"SELECT {string.Join(", ", request.SelectedColumns)} FROM Transactions";
               // List<dynamic> transactions
                List<Transaction> transactions = new List<Transaction>();
                using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    await connection.OpenAsync();
                  
                    transactions = (await connection.QueryAsync<Transaction>(query)).ToList();
                }

                if (transactions == null || !transactions.Any())
                {
                    return BadRequest("No transactions found for export.");
                }
                fileName = fileName + ".csv";
                _csvService.WriteToCSV(transactions, fileName);
                var fileBytes = await System.IO.File.ReadAllBytesAsync(fileName);
                _logger.LogInformation("Data inserted successfully to CSV.");
                return File(fileBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while exporting transactions to CSV: {ex.Message}");
                return StatusCode(500, "An error occurred while exporting transactions to CSV.");
            }
        }

        
        [HttpGet("GetTransactionsForCurrentUserTimeZone")]
        public async Task<ActionResult> GetTransactionsForCurrentUserTimeZone(DateTime startDate, DateTime endDate)
        {
            try
            {
                var transactions = await _databaseService.GetTransactionsByYearAsync(startDate, endDate);

                var timeZoneCurrentUser = TimeZoneInfo.Local.BaseUtcOffset.TotalHours;

                var filteredTransactions = await _transactionService.GetTransactionsByUserTimeZoneAsync(transactions, timeZoneCurrentUser);

                return Ok(filteredTransactions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting transactions: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching transactions.");
            }
        }

        [HttpGet("GetTransactionsForCurrentClientTimeZone")]
        public async Task<ActionResult> GetTransactionsForCurrentClientTimeZone(DateTime StartDate, DateTime EndDate)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    await connection.OpenAsync();

                    var query = @"
                            SELECT *
                            FROM Transactions";

                    var transactions = (await connection.QueryAsync<Transaction>(query)).ToList();

                    var filteredTransactions = await _transactionService.GetTransactionByClientTimeZoneAsync(transactions, StartDate, EndDate);

                    return Ok(filteredTransactions);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
    }
}
