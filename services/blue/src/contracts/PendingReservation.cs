﻿using System;

namespace artiso.AdsdHotel.Blue.Contracts
{
    public class PendingReservation
    {
        internal PendingReservation(
            string id,
            string orderId,
            string roomTypeId,
            DateTime start,
            DateTime end,
            DateTime createdAt)
        {
            Id = id;
            OrderId = orderId;
            RoomTypeId = roomTypeId;
            Confirmed = false;
            Start = start;
            End = end;
            CreatedAt = createdAt;
        }

        public string Id { get; }

        public string OrderId { get; }

        public string RoomTypeId { get; }

        public bool Confirmed { get; }

        public DateTime Start { get; }

        public DateTime End { get; }

        public DateTime CreatedAt { get; }
    }
}
