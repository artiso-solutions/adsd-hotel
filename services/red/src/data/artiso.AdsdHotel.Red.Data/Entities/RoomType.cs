using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace artiso.AdsdHotel.Red.Data.Entities
{
    public class RoomType
    {
        [BsonId]
        public Guid Id { get; internal set; }

        [BsonElement]
        public List<Rate> Rates { get; internal set; }

        [BsonElement]
        public string Type { get; internal set; }

        public RoomType(Guid id, string type, IEnumerable<Rate> rates)
        {
            Id = id;
            Type = type;
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