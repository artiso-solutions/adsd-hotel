using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Validation;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.ITOps.Sql;
using NServiceBus;
using static artiso.AdsdHotel.Blue.Api.CommonQueries;

namespace artiso.AdsdHotel.Blue.Api
{
    internal class AvailabilityHandler : IHandleMessages<AvailableRoomTypesRequest>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AvailabilityHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task Handle(AvailableRoomTypesRequest message, IMessageHandlerContext context)
        {
            try
            {
                Ensure.Valid(message);
            }
            catch (ValidationException validationEx)
            {
                await context.Reply(new Response<AvailableRoomTypesResponse>(validationEx));
                return;
            }

            await using var connection = await _connectionFactory.CreateAsync();

            var availableRoomTypes = await FindAvailableRoomTypesInPeriodAsync(connection, message.Start, message.End);

            await context.Reply(new Response<AvailableRoomTypesResponse>(
                new AvailableRoomTypesResponse(availableRoomTypes)));
        }
    }
}
