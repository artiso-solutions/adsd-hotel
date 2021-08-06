using System.Net.Http;
using System.Threading.Tasks;

namespace artiso.AdsdHotel.Black.Ambassador
{
    /// <summary>
    /// Factory to create a <see cref="BlackClient"/>.
    /// </summary>
    public class BlackClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes the factory with a <see cref="IHttpClientFactory"/> which is used to create a
        /// <see cref="BlackClient"/> on each call to <see cref="CreateAsync"/>.
        /// </summary>
        /// <param name="httpClientFactory"></param>
        public BlackClientFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Creates a <see cref="BlackClient"/> with a new <see cref="HttpClient"/> from the <see cref="IHttpClientFactory"/>.
        /// </summary>
        public async Task<BlackClient> CreateAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();
            return new BlackClient(httpClient);
        }
    }
}
