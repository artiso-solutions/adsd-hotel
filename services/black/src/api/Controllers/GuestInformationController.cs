using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.DatabaseModel;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Contracts;
using artiso.AdsdHotel.ITOps.NoSql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GuestInformationController : ControllerBase
    {
        private readonly IDataStoreClient _dataStoreClient;
        private readonly ILogger<GuestInformationController> _logger;
        private readonly IMessageSession _session;

        public GuestInformationController(IDataStoreClient dataStoreClient, ILogger<GuestInformationController> logger, IMessageSession session)
        {
            _dataStoreClient = dataStoreClient;
            _logger = logger;
            _session = session;
        }

        [HttpGet]
        public async Task<ActionResult<GuestInformationResponse>> Get(Guid orderId)
        {
            var result = await _dataStoreClient.GetAsync<GuestInformationRecord?>(ExpressionCombinationOperator.And, r => r!.OrderId == orderId);
            return result switch
            {
                not null => new GuestInformationResponse(result.GuestInformation),
                _ => NotFound($"No matching entry found for order '{orderId}'.")
            };
        }

        [HttpPost]
        public async Task<IActionResult> Set(SetGuestInformation request)
        {
            Ensure.IsValid(request.GuestInformation);
            await _session.SendLocal(request).ConfigureAwait(false);
            return Ok();
        }
    }
}
