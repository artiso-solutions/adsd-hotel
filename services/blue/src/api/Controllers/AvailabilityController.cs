using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Api.Models;
using artiso.AdsdHotel.Blue.Contracts;
using artiso.AdsdHotel.Blue.Validation;
using artiso.AdsdHotel.ITOps.Sql;
using Microsoft.AspNetCore.Mvc;
using static artiso.AdsdHotel.Blue.Api.CommonQueries;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace artiso.AdsdHotel.Blue.Api.Controllers
{
    [Route("api/availability")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        [HttpPost("search")]
        [ProducesResponseType(typeof(IReadOnlyList<RoomType>), Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyList<RoomType>>> SearchAvailability(
            [FromServices] IDbConnectionFactory connectionFactory,
            [FromBody] AvailableRoomTypesRequest request)
        {
            Ensure.Valid(request);

            await using var connection = await connectionFactory.CreateAsync();

            var availableRoomTypes = await FindAvailableRoomTypesInPeriodAsync(connection, request.Start, request.End);

            return Ok(availableRoomTypes);
        }
    }
}
