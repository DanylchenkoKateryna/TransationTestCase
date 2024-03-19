namespace TransactionTestCase.Contracts
{
    public interface ITimeZoneService
    {
        Task<string> GetTimeZoneAsync(string latitude, string longitude);
    }
}
