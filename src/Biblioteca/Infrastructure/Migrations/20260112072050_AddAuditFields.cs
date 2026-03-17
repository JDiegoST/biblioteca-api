using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Usuarios",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Usuarios",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Usuarios",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Prestamos",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Prestamos",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Prestamos",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Libros",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Libros",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Libros",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Ejemplares",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Ejemplares",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Ejemplares",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Autores",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Autores",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Autores",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_DeletedAt",
                table: "Usuarios",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_DeletedAt",
                table: "Prestamos",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_DeletedAt",
                table: "Libros",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Ejemplares_DeletedAt",
                table: "Ejemplares",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Autores_DeletedAt",
                table: "Autores",
                column: "DeletedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_DeletedAt",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Prestamos_DeletedAt",
                table: "Prestamos");

            migrationBuilder.DropIndex(
                name: "IX_Libros_DeletedAt",
                table: "Libros");

            migrationBuilder.DropIndex(
                name: "IX_Ejemplares_DeletedAt",
                table: "Ejemplares");

            migrationBuilder.DropIndex(
                name: "IX_Autores_DeletedAt",
                table: "Autores");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Ejemplares");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Ejemplares");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Ejemplares");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Autores");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Autores");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Autores");
        }
    }
}
