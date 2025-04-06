using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerSys.Backend.Migrations
{
    /// <inheritdoc />
    public partial class Correr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RolUsuario",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RolUsuario",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
