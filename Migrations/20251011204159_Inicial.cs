using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAdminEmployed.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DbEmpleado",
                columns: table => new
                {
                    UUID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IDENTIFICACION = table.Column<int>(type: "int", nullable: false),
                    PRIMER_NOMBRE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SEGUNDO_NOMBRE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PRIMER_APELLIDO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SEGUNDO_APELLIDO = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbEmpleado", x => x.UUID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbEmpleado");
        }
    }
}
