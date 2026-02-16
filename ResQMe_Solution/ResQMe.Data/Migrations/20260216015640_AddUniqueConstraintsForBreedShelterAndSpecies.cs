using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResQMe.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintsForBreedShelterAndSpecies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Species_Name",
                table: "Species",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shelters_Name_Address",
                table: "Shelters",
                columns: new[] { "Name", "Address" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Breeds_Name_SpeciesId",
                table: "Breeds",
                columns: new[] { "Name", "SpeciesId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Species_Name",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Shelters_Name_Address",
                table: "Shelters");

            migrationBuilder.DropIndex(
                name: "IX_Breeds_Name_SpeciesId",
                table: "Breeds");
        }
    }
}
