namespace artiso.AdsdHotel.Blue.Contracts
{
    public class Room
    {
        internal Room(string id, string roomTypeId, string number)
        {
            Id = id;
            RoomTypeId = roomTypeId;
            Number = number;
        }

        public string Id { get; }

        public string RoomTypeId { get; }

        public string Number { get; }
    }
}
