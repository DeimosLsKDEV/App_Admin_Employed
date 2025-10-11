using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAdminEmployed.Migrations
{
    /// <inheritdoc />
    public partial class Crear_Tabla_Empleado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    UUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDENTIFICACION = table.Column<int>(type: "int", nullable: false),
                    PRIMER_NOMBRE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SEGUNDO_NOMBRE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PRIMER_APELLIDO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SEGUNDO_APELLIDO = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.UUID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Empleados");
        }
    }
}
