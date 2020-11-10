using FluentMigrator;
using static artiso.AdsdHotel.Blue.Api.DatabaseTableNames;

namespace artiso.AdsdHotel.Blue.Api
{
    [Migration(4, TransactionBehavior.Default, "Create a view for displaying the room types and their bed composition")]
    public class ViewAvailability : Migration
    {
        public override void Up()
        {
            var statement = $@"
CREATE VIEW `{V_RoomTypes}` AS
SELECT available_rt.*, bt.Id AS `BedType.Id`, bt.InternalName AS `BedType.InternalName`, bt.Width, bt.Length
FROM {BedTypes} bt
INNER JOIN (
	SELECT rt.Id, rt.InternalName, rt.Capacity
	FROM {RoomTypes} rt
) as `available_rt`
INNER JOIN {BedTypesInRoomTypes} nm ON nm.RoomTypeId = available_rt.Id AND nm.BedTypeId = bt.Id";

            Execute.Sql(statement);
        }

        public override void Down()
        {
            // Not implemented.
        }
    }
}
