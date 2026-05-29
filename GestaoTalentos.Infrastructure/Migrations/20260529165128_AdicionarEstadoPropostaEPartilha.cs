using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestaoTalentos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarEstadoPropostaEPartilha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Propostas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TalentoSelecionadoId",
                table: "Propostas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PartilhaTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    PropostaId = table.Column<int>(type: "integer", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiraEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartilhaTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartilhaTokens_Propostas_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Propostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartilhaTokens_PropostaId",
                table: "PartilhaTokens",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_PartilhaTokens_Token",
                table: "PartilhaTokens",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartilhaTokens");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Propostas");

            migrationBuilder.DropColumn(
                name: "TalentoSelecionadoId",
                table: "Propostas");
        }
    }
}
