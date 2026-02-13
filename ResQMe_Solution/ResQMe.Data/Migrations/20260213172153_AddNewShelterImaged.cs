using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResQMe.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewShelterImaged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Shelters",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://i.ibb.co/Hf3QV5Kx/Furry-Friends-Refuge-AI.png");

            migrationBuilder.UpdateData(
                table: "Shelters",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://i.ibb.co/dsSj025K/Safe-Paws-AI.png");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Shelters",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://ibb.co/DHTnt018");

            migrationBuilder.UpdateData(
                table: "Shelters",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://ibb.co/9mJc9TrY");
        }
    }
}