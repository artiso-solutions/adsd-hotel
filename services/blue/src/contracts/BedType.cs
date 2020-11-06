namespace artiso.AdsdHotel.Blue.Contracts
{
    public class BedType
    {
        internal BedType(string id, string name, double width, double length)
        {
            Id = id;
            Name = name;
            Width = width;
            Length = length;
        }

        public string Id { get; }

        public string Name { get; }

        public double Width { get; }

        public double Length { get; }
    }
}
