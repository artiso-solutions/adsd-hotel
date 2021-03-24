using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace artiso.AdsdHotel.Red.Persistence.Entities
{
    public class RoomType
    {
        [BsonId]
        public Guid Id { get; internal set; }

        [BsonElement]
        public List<RateItem> Rates { get; internal set; }

        [BsonElement]
        public string Type { get; internal set; }

        public ConfirmationDetails ConfirmationDetails { get; internal set; }

        public RoomType(Guid id, string type, IEnumerable<RateItem> rates, ConfirmationDetails confirmationDetails)
        {
            Id = id;
            Type = type;
            ConfirmationDetails = confirmationDetails;
            Rates = rates.ToList();
        }

        [BsonIgnore]
        public float Price
        {
            get
            {
                return Rates.Select(rate => rate.Price).Sum();
            }
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Type)}: {Type}, {nameof(Rates)}: {Rates}, {nameof(Price)}: {Price}";
        }
    }
}
