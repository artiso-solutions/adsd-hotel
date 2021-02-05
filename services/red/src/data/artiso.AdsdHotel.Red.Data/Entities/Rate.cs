using System;

namespace artiso.AdsdHotel.Red.Data.Entities
{
    public class Rate
    {
        public Guid Id { get; internal set; }

        public float Price { get; internal set; }

        public Rate(Guid id, float price)
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