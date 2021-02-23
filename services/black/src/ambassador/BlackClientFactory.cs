using System;
using System.Net.Http;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Abstraction.NServiceBus;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Ambassador
{
    public class BlackClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BlackClientFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<BlackClient> CreateAsync()
        {
            var endpointConfiguration = NServiceBusEndpointConfigurationFactory.Create(
                endpointName: $"Black.Client.{Guid.NewGuid()}",
                rabbitMqConnectionString: "host=localhost");

            var endpoint = await Endpoint.Start(endpointConfiguration);
            var httpClient = _httpClientFactory.CreateClient();
            return new BlackClient(endpoint, httpClient);
        }
    }
}
