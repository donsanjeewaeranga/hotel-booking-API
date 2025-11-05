using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelReservation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomTypeDescriptionsAndAddImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "RoomTypes",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "RoomTypeId",
                keyValue: 1,
                columns: new[] { "Description", "ImageUrl" },
                values: new object[] { "A Standard Room offers comfort and convenience with essential amenities like a cozy bed, private bathroom, air conditioning, and Wi-Fi—ideal for both business and leisure stays.", "/images/standard-room.jpg" });

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "RoomTypeId",
                keyValue: 2,
                columns: new[] { "Description", "ImageUrl" },
                values: new object[] { "A Deluxe Room offers extra space, stylish décor, and upgraded amenities for a more comfortable and luxurious stay.", "/images/delux-room.jpg" });

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "RoomTypeId",
                keyValue: 3,
                columns: new[] { "Description", "ImageUrl" },
                values: new object[] { "A Suite Room provides a separate living area, premium furnishings, and exclusive amenities for an indulgent experience.", "/images/suite-room.jpg" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "RoomTypes");

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "RoomTypeId",
                keyValue: 1,
                column: "Description",
                value: "Standard room with basic amenities");

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "RoomTypeId",
                keyValue: 2,
                column: "Description",
                value: "Deluxe room with enhanced amenities");

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "RoomTypeId",
                keyValue: 3,
                column: "Description",
                value: "Luxury suite with premium amenities");
        }
    }
}
