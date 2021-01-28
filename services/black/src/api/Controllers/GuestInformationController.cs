using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.DatabaseModel;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Infrastructure.DataStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace artiso.AdsdHotel.Black.Api.Controllers
{
    [ApiController]
    public class GuestInformationController : ControllerBase
    {
        private readonly IDataStoreClient _dataStoreClient;
        private readonly ILogger<GuestInformationController> _logger;

        public GuestInformationController(IDataStoreClient dataStoreClient, ILogger<GuestInformationController> logger)
        {
            _dataStoreClient = dataStoreClient;
            _logger = logger;
        }
     
        [HttpGet]
        public async Task<ActionResult<GuestInformationResponse>> Get(Guid? orderId)
        {
            if (orderId == null)
                return BadRequest();

            var result = await _dataStoreClient.GetAsync<GuestInformationRecord?>(ExpressionCombinationOperator.And, r => r!.OrderId == orderId);
            if (result == null)
            {
                _logger.LogWarning($"No matching entry found for order '{orderId}'.");
                return NotFound($"No matching entry found for order '{orderId}'.");
            }
            else
            {
                return new GuestInformationResponse { GuestInformation = result.GuestInformation };
            }
        }
    }
}
