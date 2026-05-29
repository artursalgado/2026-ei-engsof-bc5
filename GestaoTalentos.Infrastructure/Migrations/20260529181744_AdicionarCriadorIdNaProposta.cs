using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoTalentos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCriadorIdNaProposta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CriadorId",
                table: "Propostas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Propostas_CriadorId",
                table: "Propostas",
                column: "CriadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Propostas_Users_CriadorId",
                table: "Propostas",
                column: "CriadorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Propostas_Users_CriadorId",
                table: "Propostas");

            migrationBuilder.DropIndex(
                name: "IX_Propostas_CriadorId",
                table: "Propostas");

            migrationBuilder.DropColumn(
                name: "CriadorId",
                table: "Propostas");
        }
    }
}
