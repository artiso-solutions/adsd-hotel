using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Api.Models;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Validation;
using artiso.AdsdHotel.ITOps.Sql;
using Microsoft.AspNetCore.Mvc;
using NServiceBus;
using static artiso.AdsdHotel.Blue.Api.CommonQueries;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace artiso.AdsdHotel.Blue.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class RoomNumberController : ControllerBase
    {
        [HttpGet]
        [Route("order/{orderId}/room-number")]
        [ProducesResponseType(typeof(GetRoomNumberReply), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(Status422UnprocessableEntity)]
        public async Task<ActionResult<GetRoomNumberReply>> GetRoomNumber(
            [FromServices] IDbConnectionFactory connectionFactory,
            [FromRoute] string orderId)
        {
            Ensure.Valid(orderId, nameof(orderId));

            await using var connection = await connectionFactory.CreateAsync();

            var reservation = await FindReservationAsync(connection, orderId);

            if (reservation is null)
                return NotFound($"Could not find a reservation with {nameof(orderId)} {orderId}");

            if (reservation.RoomId is not null)
            {
                var room = await FindRoomAsync(connection, reservation.RoomId);

                if (room is not null)
                    // A room was already set on this reservation.
                    return Ok(new GetRoomNumberReply(room.Number));
            }

            return UnprocessableEntity($"No room number assigned to order {orderId}");
        }

        [HttpPost]
        [Route("order/{orderId}/room-number/assign")]
        [ProducesResponseType(Status202Accepted)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<ActionResult> SetRoomNumber(
            [FromServices] IMessageSession messageSession,
            [FromRoute] string orderId)
        {
            Ensure.Valid(orderId, nameof(orderId));

            await messageSession.SendLocal(new SetRoomNumber(orderId));

            return Accepted();
        }
    }
}
