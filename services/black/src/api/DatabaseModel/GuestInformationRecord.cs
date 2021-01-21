using System;
using artiso.AdsdHotel.Black.Contracts;
using MongoDB.Bson.Serialization.Attributes;

namespace artiso.AdsdHotel.Black.Api.DatabaseModel
{
    internal class GuestInformationRecord
    {
        public GuestInformationRecord(Guid orderId, GuestInformation guestInformation)
        {
            OrderId = orderId;
            GuestInformation = guestInformation ?? throw new ArgumentNullException(nameof(guestInformation));
        }

        [BsonId]
        public Guid OrderId { get; set; }

        public GuestInformation GuestInformation { get; set; }
    }
}
