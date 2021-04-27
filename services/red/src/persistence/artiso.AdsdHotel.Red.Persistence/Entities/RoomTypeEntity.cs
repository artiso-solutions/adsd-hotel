using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace artiso.AdsdHotel.Red.Persistence.Entities
{
    public class RoomTypeEntity
    {
        [BsonId]
        public Guid Id { get; internal set; }

        [BsonElement]
        public List<RateItemEntity> Rates { get; internal set; }

        [BsonElement]
        public string Type { get; internal set; }

        [BsonElement]
        public ConfirmationDetailsEntity ConfirmationDetailsEntity { get; internal set; }

        public RoomTypeEntity()
        {
        }

        public RoomTypeEntity(Guid id, string type, IEnumerable<RateItemEntity> rates, ConfirmationDetailsEntity confirmationDetailsEntity)
        {
            Id = id;
            Type = type;
            ConfirmationDetailsEntity = confirmationDetailsEntity;
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
