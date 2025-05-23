using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pb304PetShop.Migrations
{
    /// <inheritdoc />
    public partial class Tets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "Email" },
                values: new object[] { 23, "hellomello" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 23);
        }
    }
}
