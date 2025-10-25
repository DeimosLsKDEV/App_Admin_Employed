using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAdminEmployed.Migrations
{
    /// <inheritdoc />
    public partial class ADD_CREATE_UPDATE_EMPLEADO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CREATEDAT",
                table: "Empleados",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UPDATEDAT",
                table: "Empleados",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CREATEDAT",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "UPDATEDAT",
                table: "Empleados");
        }
    }
}
