using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using NServiceBus;
using RepoDb;
using static artiso.AdsdHotel.Blue.Api.DatabaseTableNames;

namespace artiso.AdsdHotel.Blue.Api
{
    internal class AvailabilityHandler : IHandleMessages<RequestAvailableRoomTypes>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AvailabilityHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task Handle(RequestAvailableRoomTypes message, IMessageHandlerContext context)
        {
            await using var connection = await _connectionFactory.CreateAsync();

            var query = $@"
SELECT Id, InternalName, Capacity, `BedType.Id`, `BedType.InternalName`, `Width`, `Length`
FROM {V_RoomTypes} AS vrt
WHERE vrt.Id NOT IN (
	SELECT DISTINCT RoomTypeId FROM {Reservations}
	WHERE Start >= @Start AND Start <= @End)";

            using var reader = await connection.ExecuteReaderAsync(query, new { message.Start, message.End });

            var queryModels = ReadQueryModels(reader);
            var availableRoomTypes = queryModels.Select(QueryModelMapper.Map).ToList();

            await context.Reply(new AvailableRoomTypesResponse { RoomTypes = availableRoomTypes });
        }

        private IReadOnlyList<RoomTypeQueryModel> ReadQueryModels(IDataReader reader)
        {
            var stash = new Dictionary<string, RoomTypeQueryModel>();

            while (reader.Read())
            {
                var roomTypeQueryModel = ReadRoomTypeIfNew();

                var bedTypeQueryModel = new BedTypeQueryModel
                {
                    Id = reader.GetString(4),
                    InternalName = reader.GetString(5),
                    Width = reader.GetDouble(6),
                    Length = reader.GetDouble(7),
                };

                roomTypeQueryModel.BedTypes.Add(bedTypeQueryModel);
            }

            var queryModels = stash.Values.ToArray();
            return queryModels;

            // Local functions.

            RoomTypeQueryModel ReadRoomTypeIfNew()
            {
                var id = reader.GetString(0);

                if (!stash.TryGetValue(id, out var roomTypeQueryModel))
                {
                    roomTypeQueryModel = new RoomTypeQueryModel
                    {
                        Id = id,
                        InternalName = reader.GetString(1),
                        Capacity = reader.GetInt32(2)
                    };

                    stash.Add(id, roomTypeQueryModel);
                }

                return roomTypeQueryModel;
            }
        }
    }
}
