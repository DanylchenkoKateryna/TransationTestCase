using Newtonsoft.Json;
using TransactionTestCase.Contracts;
using TransactionTestCase.Models;

namespace TransactionTestCase.Services
{
    public class TimeZoneService:ITimeZoneService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TimeZoneService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
      
        public async Task<string> GetTimeZoneAsync(string latitude, string longitude)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var apiUrl = $"https://api.geotimezone.com/public/timezone?latitude={latitude}&longitude={longitude}";

                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync(); 
                var timeZoneResult = JsonConvert.DeserializeObject<TimeZoneApiResponse>(responseContent);
                var offsetValue = timeZoneResult.Offset;
                if (offsetValue != null )
                {
                    offsetValue = offsetValue.StartsWith("UTC+") ? offsetValue.Substring(4) : offsetValue.Substring(3);
                }
                return offsetValue;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching timezone from coordinates.", ex);
            }
        }
    }
}
