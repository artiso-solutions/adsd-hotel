using FluentMigrator;
using static artiso.AdsdHotel.Blue.Api.DatabaseTableNames;

namespace artiso.AdsdHotel.Blue.Api.DatabaseAccess.Migrations
{
    [Migration(3, TransactionBehavior.Default, "Populate catalog-like tables")]
    public class Catalogs : Migration
    {
        public override void Up()
        {
            Insert.IntoTable(BedTypes)
                .Row(new { Id = "BDTY-000", InternalName = "Crib", Width = 71, Length = 130 })
                .Row(new { Id = "BDTY-001", InternalName = "Single", Width = 92, Length = 190 })
                .Row(new { Id = "BDTY-002", InternalName = "Twin", Width = 99, Length = 190 })
                .Row(new { Id = "BDTY-D02", InternalName = "Double", Width = 150, Length = 188 })
                .Row(new { Id = "BDTY-K02", InternalName = "King", Width = 195, Length = 205 });

            Insert.IntoTable(RoomTypes)
                .Row(new { Id = "RMTY-001", InternalName = "Single", Capacity = 1 })
                .Row(new { Id = "RMTY-002", InternalName = "Double", Capacity = 2 })
                .Row(new { Id = "RMTY-Q04", InternalName = "Quad", Capacity = 4 })
                .Row(new { Id = "RMTY-K02", InternalName = "King", Capacity = 2 })
                .Row(new { Id = "RMTY-F05", InternalName = "Family", Capacity = 5 });

            Insert.IntoTable(BedTypesInRoomTypes)
                // Single
                .Row(new { BedTypeId = "BDTY-001", RoomTypeId = "RMTY-001" })
                // Double
                .Row(new { BedTypeId = "BDTY-D02", RoomTypeId = "RMTY-002" })
                // Quad
                .Row(new { BedTypeId = "BDTY-D02", RoomTypeId = "RMTY-Q04" })
                .Row(new { BedTypeId = "BDTY-002", RoomTypeId = "RMTY-Q04" })
                .Row(new { BedTypeId = "BDTY-002", RoomTypeId = "RMTY-Q04" })
                // King
                .Row(new { BedTypeId = "BDTY-K02", RoomTypeId = "RMTY-K02" })
                // Family
                .Row(new { BedTypeId = "BDTY-K02", RoomTypeId = "RMTY-F05" })
                .Row(new { BedTypeId = "BDTY-002", RoomTypeId = "RMTY-F05" })
                .Row(new { BedTypeId = "BDTY-002", RoomTypeId = "RMTY-F05" })
                .Row(new { BedTypeId = "BDTY-000", RoomTypeId = "RMTY-F05" });

            Insert.IntoTable(Rooms)
                // Singles
                .Row(new { Id = "RM-S01", RoomTypeId = "RMTY-001", Number = "101" })
                .Row(new { Id = "RM-S02", RoomTypeId = "RMTY-001", Number = "102" })
                .Row(new { Id = "RM-S03", RoomTypeId = "RMTY-001", Number = "201" })
                // Doubles
                .Row(new { Id = "RM-D01", RoomTypeId = "RMTY-002", Number = "103" })
                .Row(new { Id = "RM-D02", RoomTypeId = "RMTY-002", Number = "104" })
                // Quads
                .Row(new { Id = "RM-Q01", RoomTypeId = "RMTY-Q04", Number = "202" })
                // Kings
                .Row(new { Id = "RM-K01", RoomTypeId = "RMTY-K02", Number = "203" })
                // Families
                .Row(new { Id = "RM-F01", RoomTypeId = "RMTY-F05", Number = "301" });
        }

        public override void Down()
        {
            // Not implemented.
        }
    }
}
