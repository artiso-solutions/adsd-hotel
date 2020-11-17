using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Contracts;
using artiso.AdsdHotel.Blue.Validation;
using NServiceBus;
using RepoDb;
using static artiso.AdsdHotel.Blue.Api.DatabaseTableNames;

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

            var query = $@"
SELECT Id, InternalName, Capacity, `BedType.Id`, `BedType.InternalName`, `Width`, `Length`
FROM {V_RoomTypes} AS vrt
WHERE vrt.Id NOT IN (
    SELECT DISTINCT RoomTypeId FROM {Reservations}
    WHERE Start >= @Start AND Start <= @End)";

            using var reader = await connection.ExecuteReaderAsync(query, new { message.Start, message.End });

            var availableRoomTypes = await ReadRoomTypesAsync(reader);

            await context.Reply(new Response<AvailableRoomTypesResponse>(
                new AvailableRoomTypesResponse(availableRoomTypes)));
        }

        private async Task<IReadOnlyList<RoomType>> ReadRoomTypesAsync(IDataReader reader)
        {
            var stash = new Dictionary<string, (RoomType roomType, List<BedType> bedTypes)>();

            while (await reader.ReadAsync())
            {
                var (_, bedTypes) = ReadRoomTypeIfNew();

                var bedType = new BedType(
                    Id: reader.GetString(4),
                    InternalName: reader.GetString(5),
                    Width: reader.GetDouble(6),
                    Length: reader.GetDouble(7));

                bedTypes.Add(bedType);
            }

            var roomTypes = stash.Values.Select(pair => pair.roomType).ToArray();
            return roomTypes;

            // Local functions.

            (RoomType roomType, List<BedType> bedTypes) ReadRoomTypeIfNew()
            {
                var id = reader.GetString(0);

                if (stash.TryGetValue(id, out var existingRoomTypeAndBedTypes))
                    return existingRoomTypeAndBedTypes;

                var mutableBedTypes = new List<BedType>();

                var roomType = new RoomType(
                    Id: id,
                    InternalName: reader.GetString(1),
                    Capacity: reader.GetInt32(2),
                    BedTypes: mutableBedTypes);

                var newPair = (roomType, mutableBedTypes);
                stash.Add(id, newPair);

                return (roomType, mutableBedTypes);
            }
        }
    }
}
