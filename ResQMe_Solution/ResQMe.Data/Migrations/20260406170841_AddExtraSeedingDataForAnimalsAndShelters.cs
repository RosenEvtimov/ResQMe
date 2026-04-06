using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ResQMe.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExtraSeedingDataForAnimalsAndShelters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "Age", "BreedId", "BreedType", "Description", "Gender", "ImageUrl", "IsAdopted", "Name", "ShelterId", "SpeciesId" },
                values: new object[] { 4, 2, null, 2, "Playful and energetic indoor cat.", 1, "https://www.catster.com/wp-content/uploads/2023/11/Tuxedo-indoor-cat-sitting-on-the-bed_Maria-Wan_Shutterstock-800x534.jpg", true, "Buba", 2, 2 });

            migrationBuilder.InsertData(
                table: "Shelters",
                columns: new[] { "Id", "Address", "City", "Description", "Email", "ImageUrl", "Name", "Phone" },
                values: new object[,]
                {
                    { 3, "ul.Hristo Botev 42", "Varna", "Welcoming rescue center founded with the purpose of giving abandoned animals a second chance.", "hopehaven@gmail.com", "https://i.ibb.co/F4vkpv4x/Hope-Haven.jpg", "Hope Haven", "056555666" },
                    { 4, "ul.James Bourchier 85", "Sofia", "Compassionate animal shelter focused on rescuing, rehabilitating, and rehoming animals in need of care and love.", "fureverhome@gmail.com", "https://i.ibb.co/6cmWSMT9/Furever-Home.png", "Furever Home", "056777888" }
                });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "Age", "BreedId", "BreedType", "Description", "Gender", "ImageUrl", "IsAdopted", "Name", "ShelterId", "SpeciesId" },
                values: new object[] { 5, 3, null, 2, "Clingy and calm stray cat.", 1, "https://upload.wikimedia.org/wikipedia/commons/thumb/4/4d/Cat_November_2010-1a.jpg/960px-Cat_November_2010-1a.jpg", false, "Corny", 3, 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Shelters",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Shelters",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
