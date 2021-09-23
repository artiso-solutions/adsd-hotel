using System.Net.Http;

namespace artiso.AdsdHotel.Blue.Ambassador
{
    public class BlueAmbassadorFactory
    {
        private readonly IHttpClientFactory? _httpClientFactory;

        public BlueAmbassadorFactory()
        {
        }

        public BlueAmbassadorFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public BlueAmbassador Create(string apiBaseAddress)
        {
            var httpClient = _httpClientFactory?.CreateClient() ?? new HttpClient();
            var ambassador = new BlueAmbassador(apiBaseAddress, httpClient);
            return ambassador;
        }
    }
}
