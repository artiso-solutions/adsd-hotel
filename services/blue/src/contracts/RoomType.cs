using System.Collections.Generic;

namespace artiso.AdsdHotel.Blue.Contracts
{
    public class RoomType
    {
        internal RoomType(string id, string internalName, int capacity, IReadOnlyList<BedType> bedTypes)
        {
            Id = id;
            InternalName = internalName;
            Capacity = capacity;
            BedTypes = bedTypes;
        }

        public string Id { get; }

        public string InternalName { get; }

        public int Capacity { get; }

        public IReadOnlyList<BedType> BedTypes { get; }
    }
}
