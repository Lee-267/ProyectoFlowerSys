using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerSys.Backend.Migrations
{
    /// <inheritdoc />
    public partial class HistorialAccionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operaciones_Users_UsuarioId",
                table: "Operaciones");

            migrationBuilder.DropIndex(
                name: "IX_Operaciones_UsuarioId",
                table: "Operaciones");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Operaciones");

            migrationBuilder.CreateTable(
                name: "HistorialAcciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Accion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaAccion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialAcciones", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialAcciones");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Operaciones",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Operaciones_UsuarioId",
                table: "Operaciones",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operaciones_Users_UsuarioId",
                table: "Operaciones",
                column: "UsuarioId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
