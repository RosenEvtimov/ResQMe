using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResQMe.Data.Migrations
{
    /// <inheritdoc />
    public partial class AnimalImageSizeFixForCatCorny : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://images.ctfassets.net/4cuokmrwdiqv/3mwx2f0pzK0g09nQlXa3eZ/598f6f47d346e85bbc231e10c86ddc85/tabby-cat-breed-page-carousel-image-1.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/thumb/4/4d/Cat_November_2010-1a.jpg/960px-Cat_November_2010-1a.jpg");
        }
    }
}
