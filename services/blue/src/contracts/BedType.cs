namespace artiso.AdsdHotel.Blue.Contracts
{
    public class BedType
    {
        internal BedType(string id, string name, int width, int length)
        {
            Id = id;
            Name = name;
            Width = width;
            Length = length;
        }

        public string Id { get; }

        public string Name { get; }

        public int Width { get; }

        public int Length { get; }
    }
}
