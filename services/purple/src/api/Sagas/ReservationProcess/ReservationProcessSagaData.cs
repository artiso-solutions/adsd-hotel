using System;
using NServiceBus;

namespace artiso.AdsdHotel.Purple.Api.Sagas
{
    internal class ReservationProcessSagaData : IContainSagaData
    {
        public Guid Id { get; set; }

        public string? Originator { get; set; }

        public string? OriginalMessageId { get; set; }

        public string? OrderId { get; set; }

        public bool CancellationFeeAcquired { get; set; }

        public bool RoomTypeConfirmed { get; set; }

        public bool CancellationFeeCharged { get; set; }
    }
}
