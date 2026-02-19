using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetLocation.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Vehicules_VehiculeId1",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_VehiculeId1",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "VehiculeId1",
                table: "Reservations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VehiculeId1",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_VehiculeId1",
                table: "Reservations",
                column: "VehiculeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Vehicules_VehiculeId1",
                table: "Reservations",
                column: "VehiculeId1",
                principalTable: "Vehicules",
                principalColumn: "Id");
        }
    }
}
