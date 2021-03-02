using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Contracts;
using artiso.AdsdHotel.ITOps.Sql;
using RepoDb;
using static artiso.AdsdHotel.Blue.Api.DatabaseTableNames;

namespace artiso.AdsdHotel.Blue.Api
{
    internal static class CommonQueries
    {
        public static async Task<PendingReservation?> FindPendingReservationAsync(
            IDbConnection connection,
            string orderId)
        {
            var query = $@"
SELECT Id, OrderId, RoomTypeId, Start, End, CreatedAt, Confirmed
FROM {PendingReservations}
WHERE OrderId = @orderId";

            using var reader = await connection.ExecuteReaderAsync(query, new { orderId });

            if (!await reader.ReadAsync())
                return null;

            var i = 0;
            var pendingReservation = new PendingReservation(
                Id: reader.GetString(i++),
                OrderId: reader.GetString(i++),
                RoomTypeId: reader.GetString(i++),
                Start: reader.GetDateTime(i++),
                End: reader.GetDateTime(i++),
                CreatedAt: reader.GetDateTime(i++))
            {
                Confirmed = reader.GetBoolean(i++)
            };

            return pendingReservation;
        }

        public static async Task<Reservation?> FindReservationAsync(IDbConnection connection, string orderId)
        {
            var query = $@"
SELECT Id, OrderId, RoomTypeId, Start, End, CreatedAt, RoomId
FROM {Reservations}
WHERE OrderId = @orderId";

            using var reader = await connection.ExecuteReaderAsync(query, new { orderId });

            if (!await reader.ReadAsync())
                return null;

            var i = 0;
            var reservation = new Reservation(
                Id: reader.GetString(i++),
                OrderId: reader.GetString(i++),
                RoomTypeId: reader.GetString(i++),
                Start: reader.GetDateTime(i++),
                End: reader.GetDateTime(i++),
                CreatedAt: reader.GetDateTime(i++))
            {
                RoomId = reader.IsDBNull(i) ? null : reader.GetString(i++)
            };

            return reservation;
        }

        public static async Task<Room?> FindRoomAsync(IDbConnection connection, string roomId)
        {
            var query = $@"
SELECT Id, RoomTypeId, Number
FROM {Rooms}
WHERE OrderId = @roomId";

            using var reader = await connection.ExecuteReaderAsync(query, new { roomId });

            if (!await reader.ReadAsync())
                return null;

            var i = 0;
            var room = new Room(
                Id: reader.GetString(i++),
                RoomTypeId: reader.GetString(i++),
                Number: reader.GetString(i++));

            return room;
        }

        public static async Task<RoomType?> FindRoomTypeAsync(
            IDbConnection connection,
            string roomTypeId)
        {
            var query = $@"
SELECT Id, InternalName, Capacity, `BedType.Id`, `BedType.InternalName`, `Width`, `Length`
FROM {V_RoomTypes} AS vrt
WHERE vrt.Id = @roomTypeId";

            using var reader = await connection.ExecuteReaderAsync(query, new { roomTypeId });

            var roomTypes = await ReadRoomTypesAsync(reader);

            return roomTypes[0];
        }

        public static async Task<IReadOnlyList<RoomType>> FindAvailableRoomTypesInPeriodAsync(
            IDbConnection connection,
            DateTime start,
            DateTime end)
        {
            var query = $@"
SELECT Id, InternalName, Capacity, `BedType.Id`, `BedType.InternalName`, `Width`, `Length`
FROM {V_RoomTypes} AS vrt
WHERE vrt.Id NOT IN (
    SELECT DISTINCT RoomTypeId FROM {Reservations}
    WHERE Start >= @start AND Start <= @end)";

            using var reader = await connection.ExecuteReaderAsync(query, new { start, end });

            var availableRoomTypes = await ReadRoomTypesAsync(reader);

            return availableRoomTypes;
        }

        private static async Task<IReadOnlyList<RoomType>> ReadRoomTypesAsync(IDataReader reader)
        {
            var stash = new Dictionary<string, (RoomType roomType, List<BedType> bedTypes)>();

            while (await reader.ReadAsync())
            {
                var (_, bedTypes) = ReadRoomTypeIfNew();

                var bedType = new BedType(
                    Id: reader.GetString(3),
                    InternalName: reader.GetString(4),
                    Width: reader.GetDouble(5),
                    Length: reader.GetDouble(6));

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
