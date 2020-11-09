namespace artiso.AdsdHotel.Red.Data.Entities
{
    public class Rate
    {
        public string Name { get; internal set; }

        public float Price { get; internal set; }

        internal Rate(string name, float price)
        {
            Name = name;
            Price = price;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Price)}: {Price}";
        }
    }
}