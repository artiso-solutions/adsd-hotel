using FluentMigrator;
using static artiso.AdsdHotel.Blue.Api.DatabaseTableNames;

namespace artiso.AdsdHotel.Blue.Api
{
    [Migration(1, TransactionBehavior.Default, "Setup of the database tables and overall schema")]
    public class Schemas : Migration
    {
        private const int _smallSize = 40;
        private const int _normalSize = 200;

        public override void Up()
        {
            Create.Table(BedTypes)
                .WithColumn("Id").AsString(_smallSize).NotNullable().PrimaryKey()
                .WithColumn("InternalName").AsString(_normalSize).NotNullable()
                .WithColumn("Width").AsDouble().NotNullable()
                .WithColumn("Length").AsDouble().NotNullable();

            Create.Table(RoomTypes)
                .WithColumn("Id").AsString(_smallSize).NotNullable().PrimaryKey()
                .WithColumn("InternalName").AsString(_normalSize).NotNullable()
                .WithColumn("Capacity").AsInt32().NotNullable();

            Create.Table(BedTypesInRoomTypes)
                .WithColumn("BedTypeId").AsString(_smallSize).NotNullable()
                .WithColumn("RoomTypeId").AsString(_smallSize).NotNullable();

            Create.Table(Rooms)
                .WithColumn("Id").AsString(_smallSize).NotNullable().PrimaryKey()
                .WithColumn("RoomTypeId").AsString(_smallSize).NotNullable()
                .WithColumn("Number").AsString(_smallSize).NotNullable();

            Create.Table(PendingReservations)
                .WithColumn("Id").AsString(_smallSize).NotNullable().PrimaryKey()
                .WithColumn("OrderId").AsString(_normalSize).NotNullable()
                .WithColumn("RoomTypeId").AsString(_smallSize).NotNullable()
                .WithColumn("Confirmed").AsBoolean().NotNullable()
                .WithColumn("Start").AsDateTime().NotNullable()
                .WithColumn("End").AsDateTime().NotNullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable();

            Create.Table(Reservations)
                .WithColumn("Id").AsString(_smallSize).NotNullable().PrimaryKey()
                .WithColumn("OrderId").AsString(_normalSize).NotNullable()
                .WithColumn("RoomTypeId").AsString(_smallSize).NotNullable()
                .WithColumn("RoomId").AsString(_smallSize).Nullable()
                .WithColumn("Start").AsDateTime().NotNullable()
                .WithColumn("End").AsDateTime().NotNullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable();
        }

        public override void Down()
        {
            // Not implemented.
        }
    }
}
