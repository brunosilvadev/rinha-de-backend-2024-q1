using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rinha.Migrations
{
    /// <inheritdoc />
    public partial class saldo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SaldoInicial",
                table: "Clientes",
                newName: "Saldo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Saldo",
                table: "Clientes",
                newName: "SaldoInicial");
        }
    }
}
