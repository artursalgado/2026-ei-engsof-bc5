using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestaoTalentos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerfisAndPropostasAndTalentosElegiveis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Perfis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Pais = table.Column<string>(type: "text", nullable: false),
                    PrecoPorHora = table.Column<decimal>(type: "numeric", nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    IsShared = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Propostas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AreaId = table.Column<int>(type: "integer", nullable: false),
                    DescricaoTrabalho = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    NumeroTotalHoras = table.Column<int>(type: "integer", nullable: false),
                    PrecoHoraMedio = table.Column<decimal>(type: "numeric", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propostas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Propostas_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkillsNecessarias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NivelMinimoRequerido = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false),
                    PropostaId = table.Column<int>(type: "integer", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillsNecessarias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillsNecessarias_Propostas_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Propostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkillsNecessarias_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TalentosElegiveis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropostaId = table.Column<int>(type: "integer", nullable: false),
                    PerfilId = table.Column<int>(type: "integer", nullable: false),
                    ValorEstimado = table.Column<decimal>(type: "numeric", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalentosElegiveis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TalentosElegiveis_Perfis_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "Perfis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TalentosElegiveis_Propostas_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Propostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Propostas_AreaId",
                table: "Propostas",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Propostas_Nome",
                table: "Propostas",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillsNecessarias_PropostaId",
                table: "SkillsNecessarias",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillsNecessarias_SkillId",
                table: "SkillsNecessarias",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_TalentosElegiveis_PerfilId",
                table: "TalentosElegiveis",
                column: "PerfilId");

            migrationBuilder.CreateIndex(
                name: "IX_TalentosElegiveis_PropostaId",
                table: "TalentosElegiveis",
                column: "PropostaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "SkillsNecessarias");
            migrationBuilder.DropTable(name: "TalentosElegiveis");
            migrationBuilder.DropTable(name: "Perfis");
            migrationBuilder.DropTable(name: "Propostas");
        }
    }
}