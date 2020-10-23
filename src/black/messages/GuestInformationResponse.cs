using System;
using System.Collections.Generic;
using System.Text;
using artiso.AdsdHotel.Black.Contracts;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Messages
{
    public class GuestInformationResponse : IMessage
    {
        public GuestInformation GuestInformation{ get; set; }
    }
}
