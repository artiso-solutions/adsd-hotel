namespace artiso.AdsdHotel.Blue.Contracts
{
    public class BedType
    {
        internal BedType(string id, string internalName, double width, double length)
        {
            Id = id;
            InternalName = internalName;
            Width = width;
            Length = length;
        }

        public string Id { get; }

        public string InternalName { get; }

        public double Width { get; }

        public double Length { get; }
    }
}
