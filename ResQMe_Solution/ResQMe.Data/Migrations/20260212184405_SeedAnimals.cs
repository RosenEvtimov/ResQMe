using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ResQMe.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedAnimals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "Age", "BreedId", "BreedType", "Description", "Gender", "ImageUrl", "IsAdopted", "Name", "ShelterId", "SpeciesId" },
                values: new object[,]
                {
                    { 1, 3, 3, 0, "Friendly and energetic dog.", 0, "https://www.borrowmydoggy.com/_next/image?url=https%3A%2F%2Fcdn.sanity.io%2Fimages%2F4ij0poqn%2Fproduction%2Fda89d930fc320dd912a2a25487b9ca86b37fcdd6-800x600.jpg&w=640&q=80", false, "Max", 1, 1 },
                    { 2, 2, null, 1, "Calm indoor cat.", 1, "https://www.trupanion.com/images/trupanionwebsitelibraries/bg/mixed-breed-cat.jpg?sfvrsn=959c7b73_1", false, "Bella", 2, 2 },
                    { 3, 5, null, 2, "Rescued stray dog.", 0, "https://media.4-paws.org/8/5/5/4/85545b5798b851ea1cf81552bc56a49d96a71882/VIER%20PFOTEN_2023-10-19_00151-2850x1900-2746x1900-1920x1328.jpg", true, "Rocky", 1, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
