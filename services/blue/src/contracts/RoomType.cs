using System.Collections.Generic;

namespace artiso.AdsdHotel.Blue.Contracts
{
    public record RoomType(string Id, string InternalName, int Capacity, IReadOnlyList<BedType> BedTypes);
}
