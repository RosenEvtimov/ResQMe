using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResQMe.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddShelterDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Shelters",
                type: "nvarchar(750)",
                maxLength: 750,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Shelters",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Volunteer-run shelter dedicated to rescuing abandoned animals in Burgas.");

            migrationBuilder.UpdateData(
                table: "Shelters",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Non-profit organisation dedicated to fostering homeless animals in Burgas.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Shelters");
        }
    }
}
