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
                name: "Perfil",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    IsShared = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfil", x => x.Id);
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
                        name: "FK_TalentosElegiveis_Perfil_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "Perfil",
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
                name: "IX_Areas_Nome",
                table: "Areas",
                column: "Nome",
                unique: true);

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
                name: "IX_Skills_AreaId",
                table: "Skills",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_Nome",
                table: "Skills",
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
            migrationBuilder.DropTable(
                name: "SkillsNecessarias");

            migrationBuilder.DropTable(
                name: "TalentosElegiveis");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Perfil");

            migrationBuilder.DropTable(
                name: "Propostas");

            migrationBuilder.DropTable(
                name: "Areas");
        }
    }
}
