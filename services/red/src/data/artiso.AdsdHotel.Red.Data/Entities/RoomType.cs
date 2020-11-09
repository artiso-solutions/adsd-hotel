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
        public string Name { get; internal set; }

        [BsonElement]
        public List<Rate> Rates { get; internal set; }

        internal RoomType(string name, IEnumerable<Rate> rates)
        {
            Name = name;
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
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Rates)}: {Rates}, {nameof(Price)}: {Price}";
        }
    }
}