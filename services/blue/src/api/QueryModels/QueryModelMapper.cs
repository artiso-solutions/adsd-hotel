using System.Linq;
using artiso.AdsdHotel.Blue.Contracts;

namespace artiso.AdsdHotel.Blue.Api
{
    internal class QueryModelMapper
    {
        public static RoomType Map(RoomTypeQueryModel qm)
        {
            return new RoomType(
                qm.Id ?? string.Empty,
                qm.InternalName ?? string.Empty,
                qm.Capacity ?? 0,
                qm.BedTypes.Select(Map).ToArray());
        }

        public static BedType Map(BedTypeQueryModel qm)
        {
            return new BedType(
                qm.Id ?? string.Empty,
                qm.InternalName ?? string.Empty,
                qm.Width ?? 0,
                qm.Length ?? 0);
        }
    }
}
