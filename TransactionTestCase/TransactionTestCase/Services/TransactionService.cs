using TransactionTestCase.Contracts;
using TransactionTestCase.Entity;

public class TransactionService : ITransactionService
{
    private readonly ITimeZoneService _timeZoneService;

    public TransactionService(ITimeZoneService timeZoneService)
    {
        _timeZoneService = timeZoneService ?? throw new ArgumentNullException(nameof(timeZoneService)); 
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByUserTimeZoneAsync(IEnumerable<Transaction> transactions, double timeZoneCurrentUser)
    {
        var filteredTransactions = new List<Transaction>();
        var tasks = new List<Task<string>>();
        foreach (var transaction in transactions)
        {
            string[] coordinates = transaction.ClientLocation.Split(',');
            string latitude = coordinates[0];
            string longitude = coordinates[1];

            Task<string> offSetClientTask = _timeZoneService.GetTimeZoneAsync(latitude, longitude);
            tasks.Add(offSetClientTask);

        }
        var results = await Task.WhenAll(tasks);

        for (int i = 0; i < results.Length; i++)
        {
            if (results[i] != null && results[i] == timeZoneCurrentUser.ToString())
            {
                filteredTransactions.Add(transactions.ElementAt(i));
            }
        }
        return filteredTransactions;
    }

    public async Task<IEnumerable<Transaction>> GetTransactionByClientTimeZoneAsync(IEnumerable<Transaction> transactions, DateTime startDate, DateTime endDate)
    {
        var filteredTransactions = new List<Transaction>();
        var tasks = new List<Task<string>>(); 

        foreach (var transaction in transactions)
        {
            string[] coordinates = transaction.ClientLocation.Split(',');
            string latitude = coordinates[0];
            string longitude = coordinates[1];

            Task<string> offSetClientTask = _timeZoneService.GetTimeZoneAsync(latitude, longitude);
            tasks.Add(offSetClientTask);
        }

        await Task.WhenAll(tasks);

        for (int i = 0; i < transactions.Count(); i++)
        {
            var transaction = transactions.ElementAt(i);
            string offSetClient = await tasks[i]; 

            if (int.TryParse(offSetClient, out int offsetHours))
            {
                TimeSpan offset = TimeSpan.FromHours(offsetHours);

                var timeZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(tz => tz.BaseUtcOffset == offset);
                var convertedStartDate = TimeZoneInfo.ConvertTime(startDate, timeZone);
                var convertedEndDate = TimeZoneInfo.ConvertTime(endDate, timeZone);

                if (transaction.TransactionDate >= convertedStartDate && transaction.TransactionDate <= convertedEndDate)
                {
                    filteredTransactions.Add(transaction);
                }
            }
        }

        return filteredTransactions;
    }
}