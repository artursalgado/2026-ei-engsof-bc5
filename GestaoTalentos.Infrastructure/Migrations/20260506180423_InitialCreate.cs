using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestaoTalentos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    TipoUtilizador = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
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
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AreaId = table.Column<int>(type: "integer", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skills_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    IdCriador = table.Column<int>(type: "integer", nullable: false),
                    IdMinhaConta = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clientes_Users_IdCriador",
                        column: x => x.IdCriador,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Perfil",
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
                    table.PrimaryKey("PK_Perfil", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Perfil_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "ExperienciasProfissionais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PerfilId = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Empresa = table.Column<string>(type: "text", nullable: false),
                    AnoInicio = table.Column<int>(type: "integer", nullable: false),
                    AnoFim = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienciasProfissionais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExperienciasProfissionais_Perfil_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "Perfil",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerfilSkills",
                columns: table => new
                {
                    PerfilId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false),
                    AnosExperiencia = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilSkills", x => new { x.PerfilId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_PerfilSkills_Perfil_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "Perfil",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfilSkills_Skills_SkillId",
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

            migrationBuilder.InsertData(
                table: "Areas",
                columns: new[] { "Id", "CriadoEm", "Nome" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 6, 18, 4, 23, 389, DateTimeKind.Utc).AddTicks(4780), "Developer" },
                    { 2, new DateTime(2026, 5, 6, 18, 4, 23, 389, DateTimeKind.Utc).AddTicks(4790), "Designer" },
                    { 3, new DateTime(2026, 5, 6, 18, 4, 23, 389, DateTimeKind.Utc).AddTicks(4790), "Product Manager" },
                    { 4, new DateTime(2026, 5, 6, 18, 4, 23, 389, DateTimeKind.Utc).AddTicks(4790), "Project Manager" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Areas_Nome",
                table: "Areas",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdCriador",
                table: "Clientes",
                column: "IdCriador");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienciasProfissionais_PerfilId",
                table: "ExperienciasProfissionais",
                column: "PerfilId");

            migrationBuilder.CreateIndex(
                name: "IX_Perfil_OwnerId",
                table: "Perfil",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilSkills_SkillId",
                table: "PerfilSkills",
                column: "SkillId");

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
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "ExperienciasProfissionais");

            migrationBuilder.DropTable(
                name: "PerfilSkills");

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
                name: "Users");

            migrationBuilder.DropTable(
                name: "Areas");
        }
    }
}