using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDBDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ejemplares_Libros_LibroId",
                table: "Ejemplares");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_Usuarios_UsuarioId",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "EnExistencia",
                table: "Prestamos");

            migrationBuilder.AddForeignKey(
                name: "FK_Ejemplares_Libros_LibroId",
                table: "Ejemplares",
                column: "LibroId",
                principalTable: "Libros",
                principalColumn: "LibroId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Usuarios_UsuarioId",
                table: "Prestamos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ejemplares_Libros_LibroId",
                table: "Ejemplares");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_Usuarios_UsuarioId",
                table: "Prestamos");

            migrationBuilder.AddColumn<bool>(
                name: "EnExistencia",
                table: "Prestamos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ejemplares_Libros_LibroId",
                table: "Ejemplares",
                column: "LibroId",
                principalTable: "Libros",
                principalColumn: "LibroId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Usuarios_UsuarioId",
                table: "Prestamos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
