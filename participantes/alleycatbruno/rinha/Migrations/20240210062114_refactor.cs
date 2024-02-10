using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rinha.Migrations
{
    /// <inheritdoc />
    public partial class refactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Clientes_ClienteId",
                table: "Transacoes");

            migrationBuilder.DropIndex(
                name: "IX_Transacoes_ClienteId",
                table: "Transacoes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_ClienteId",
                table: "Transacoes",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Clientes_ClienteId",
                table: "Transacoes",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
