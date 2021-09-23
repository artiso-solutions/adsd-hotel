using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Validation;
using Microsoft.AspNetCore.Mvc;
using NServiceBus;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace artiso.AdsdHotel.Blue.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class RoomTypeSelectionController : ControllerBase
    {
        [HttpPost]
        [Route("order/{orderId}/room-type/select")]
        [ProducesResponseType(Status202Accepted)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<ActionResult> SelectRoomType(
            [FromServices] IMessageSession messageSession,
            [FromRoute] string orderId,
            [FromBody] Models.SelectRoomType payload)
        {
            var command = new Commands.SelectRoomType(
                orderId,
                payload.RoomTypeId,
                payload.Start,
                payload.End);

            Ensure.Valid(command);

            await messageSession.SendLocal(command);

            return Ok();
        }
    }
}
