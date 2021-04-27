using System;

namespace artiso.AdsdHotel.Red.Persistence.Entities
{
    public class RateItemEntity
    {
        public Guid Id { get; internal set; }

        public float Price { get; internal set; }

        public RateItemEntity(Guid id, float price)
        {
            Id = id;
            Price = price;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Price)}: {Price}";
        }
    }
}
