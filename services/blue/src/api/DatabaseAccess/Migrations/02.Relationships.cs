using FluentMigrator;
using static artiso.AdsdHotel.Blue.Api.DatabaseTableNames;

namespace artiso.AdsdHotel.Blue.Api
{
    [Migration(2, TransactionBehavior.Default, "Setup of the relationships between the tables")]
    public class Relationships : Migration
    {
        public override void Up()
        {
            Create.ForeignKey("fk_BedTypesInRoomTypes_BedTypes")
                .FromTable(BedTypesInRoomTypes).ForeignColumn("BedTypeId")
                .ToTable(BedTypes).PrimaryColumn("Id");

            Create.ForeignKey("fk_BedTypesInRoomTypes_RoomTypes")
                .FromTable(BedTypesInRoomTypes).ForeignColumn("RoomTypeId")
                .ToTable(RoomTypes).PrimaryColumn("Id");

            Create.ForeignKey("fk_Rooms_RoomTypes")
                .FromTable(Rooms).ForeignColumn("RoomTypeId")
                .ToTable(RoomTypes).PrimaryColumn("Id");

            Create.ForeignKey("fk_PendingReservations_RoomTypes")
                .FromTable(PendingReservations).ForeignColumn("RoomTypeId")
                .ToTable(RoomTypes).PrimaryColumn("Id");

            Create.ForeignKey("fk_Reservations_RoomTypes")
                .FromTable(Reservations).ForeignColumn("RoomTypeId")
                .ToTable(RoomTypes).PrimaryColumn("Id");
        }

        public override void Down()
        {
            // Not implemented.
        }
    }
}
