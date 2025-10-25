using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAdminEmployed.Migrations
{
    /// <inheritdoc />
    public partial class FK_Empleado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GUID_SUPERVISOR",
                table: "Empleados",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_GUID_SUPERVISOR",
                table: "Empleados",
                column: "GUID_SUPERVISOR");

            migrationBuilder.AddForeignKey(
                name: "FK_Empleados_Empleados_GUID_SUPERVISOR",
                table: "Empleados",
                column: "GUID_SUPERVISOR",
                principalTable: "Empleados",
                principalColumn: "UUID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empleados_Empleados_GUID_SUPERVISOR",
                table: "Empleados");

            migrationBuilder.DropIndex(
                name: "IX_Empleados_GUID_SUPERVISOR",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "GUID_SUPERVISOR",
                table: "Empleados");
        }
    }
}
