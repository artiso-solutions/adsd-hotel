using System.Collections.Generic;

namespace artiso.AdsdHotel.Blue.Contracts
{
    public class RoomType
    {
        internal RoomType(string id, string name, int capacity, IReadOnlyList<BedType> bedTypes)
        {
            Id = id;
            Name = name;
            Capacity = capacity;
            BedTypes = bedTypes;
        }

        public string Id { get; }

        public string Name { get; }

        public int Capacity { get; }

        public IReadOnlyList<BedType> BedTypes { get; }
    }
}
