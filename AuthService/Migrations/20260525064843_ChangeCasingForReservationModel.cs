using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCasingForReservationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "Reservations",
                newName: "Status");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "Loans",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "100",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ef092cee-7805-44a2-9ea4-e3502203a9a7", "AQAAAAIAAYagAAAAEOqvIW/Z692oMErGsFD48FXI2UuTRcSHB++H+m4JNiQRSLkMclOe+jYcv25JxyCERw==", "dac0cdcf-f1a6-4ffb-b6dc-e70af6766d54" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Reservations",
                newName: "status");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "Loans",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "100",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8f1d4465-1657-4d10-a7b8-bf47e0c6437f", "AQAAAAIAAYagAAAAEISFADNBlr++5T9U3e8f4O3vRv01zRAeDLdRMnpROaC+eN2cP0QN+R6sBTyfdHInmg==", "bf1d27e1-f198-4a7d-ac09-aab825d1f683" });
        }
    }
}
