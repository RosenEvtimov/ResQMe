using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResQMe.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddShelterImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Shelters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Shelters");
        }
    }
}
