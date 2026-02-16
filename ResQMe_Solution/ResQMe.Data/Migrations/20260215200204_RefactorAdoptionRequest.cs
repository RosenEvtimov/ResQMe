using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResQMe.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAdoptionRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "AdoptionRequests");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AdoptionRequests");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AdoptionRequests");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "AdoptionRequests");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AdoptionRequests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionRequests_UserId",
                table: "AdoptionRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionRequests_AspNetUsers_UserId",
                table: "AdoptionRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionRequests_AspNetUsers_UserId",
                table: "AdoptionRequests");

            migrationBuilder.DropIndex(
                name: "IX_AdoptionRequests_UserId",
                table: "AdoptionRequests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AdoptionRequests");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AdoptionRequests",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AdoptionRequests",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AdoptionRequests",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "AdoptionRequests",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
