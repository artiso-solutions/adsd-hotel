using System.Collections.Generic;

namespace artiso.AdsdHotel.Blue.Api
{
    internal class RoomTypeQueryModel
    {
        public string? Id { get; set; }

        public string? InternalName { get; set; }

        public int? Capacity { get; set; }

        public List<BedTypeQueryModel> BedTypes { get; set; } = new List<BedTypeQueryModel>();
    }
}
