using System;
using artiso.AdsdHotel.Black.Contracts;

namespace artiso.AdsdHotel.Black.Api.DatabaseModel
{
    class GuestInformationRecord
    {
        public Guid OrderId { get; set; }

        public GuestInformation GuestInformation { get; set; }
    }
}
