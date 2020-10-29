using System;
using artiso.AdsdHotel.Black.Contracts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace artiso.AdsdHotel.Black.Api.DatabaseModel
{
    class GuestInformationRecord
    {
        [BsonId]
        public Guid OrderId { get; set; }

        public GuestInformation GuestInformation { get; set; }
    }
}
