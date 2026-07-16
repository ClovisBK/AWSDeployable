using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class AddOtherFieldsInBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ISBN",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "100",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8f1d4465-1657-4d10-a7b8-bf47e0c6437f", "AQAAAAIAAYagAAAAEISFADNBlr++5T9U3e8f4O3vRv01zRAeDLdRMnpROaC+eN2cP0QN+R6sBTyfdHInmg==", "bf1d27e1-f198-4a7d-ac09-aab825d1f683" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ISBN",
                table: "Books");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "100",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b3ce6919-5e77-47a7-a1ae-bc1f07e60bd5", "AQAAAAIAAYagAAAAEOgQQq8IAZeEudn+CRaZOehN2ncMajdQ2Qo4d4TffMUEXsql4ciB+PQd18FXy9Y4Tw==", "ae12f390-ca6b-4592-817d-ea39a0ea315d" });
        }
    }
}
