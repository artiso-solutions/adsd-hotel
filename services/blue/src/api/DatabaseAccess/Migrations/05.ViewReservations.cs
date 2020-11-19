using FluentMigrator;
using static artiso.AdsdHotel.Blue.Api.DatabaseTableNames;

namespace artiso.AdsdHotel.Blue.Api
{
    [Migration(5, TransactionBehavior.Default, "Create a view for displaying the availability")]
    public class ViewReservations : Migration
    {
        public override void Up()
        {
            var statement = $@"
CREATE VIEW `{V_Reservations}` AS
SELECT res.OrderId, rs.Id as `RoomId`, rs.Number as `RoomNumber`, rt.InternalName as `RoomType`, res.Start, res.End
FROM {Reservations} res
INNER JOIN {Rooms} rs ON rs.Id = res.RoomId
INNER JOIN {RoomTypes} rt ON rt.Id = res.RoomTypeId";

            Execute.Sql(statement);
        }

        public override void Down()
        {
            // Not implemented.
        }
    }
}
