using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestaoTalentos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLogsAndCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExperienciasProfissionais_Perfil_PerfilId",
                table: "ExperienciasProfissionais");

            migrationBuilder.DropForeignKey(
                name: "FK_Perfil_Users_OwnerId",
                table: "Perfil");

            migrationBuilder.DropForeignKey(
                name: "FK_PerfilSkills_Perfil_PerfilId",
                table: "PerfilSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_TalentosElegiveis_Perfil_PerfilId",
                table: "TalentosElegiveis");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Perfil",
                table: "Perfil");

            migrationBuilder.DropColumn(
                name: "TipoUtilizador",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Perfil",
                newName: "Perfis");

            migrationBuilder.RenameIndex(
                name: "IX_Perfil_OwnerId",
                table: "Perfis",
                newName: "IX_Perfis_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Perfis",
                table: "Perfis",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    CommandName = table.Column<string>(type: "text", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityName = table.Column<string>(type: "text", nullable: false),
                    OldValues = table.Column<string>(type: "text", nullable: true),
                    NewValues = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ExperienciasProfissionais_Perfis_PerfilId",
                table: "ExperienciasProfissionais",
                column: "PerfilId",
                principalTable: "Perfis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PerfilSkills_Perfis_PerfilId",
                table: "PerfilSkills",
                column: "PerfilId",
                principalTable: "Perfis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Perfis_Users_OwnerId",
                table: "Perfis",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TalentosElegiveis_Perfis_PerfilId",
                table: "TalentosElegiveis",
                column: "PerfilId",
                principalTable: "Perfis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExperienciasProfissionais_Perfis_PerfilId",
                table: "ExperienciasProfissionais");

            migrationBuilder.DropForeignKey(
                name: "FK_PerfilSkills_Perfis_PerfilId",
                table: "PerfilSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_Perfis_Users_OwnerId",
                table: "Perfis");

            migrationBuilder.DropForeignKey(
                name: "FK_TalentosElegiveis_Perfis_PerfilId",
                table: "TalentosElegiveis");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Perfis",
                table: "Perfis");

            migrationBuilder.RenameTable(
                name: "Perfis",
                newName: "Perfil");

            migrationBuilder.RenameIndex(
                name: "IX_Perfis_OwnerId",
                table: "Perfil",
                newName: "IX_Perfil_OwnerId");

            migrationBuilder.AddColumn<int>(
                name: "TipoUtilizador",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Perfil",
                table: "Perfil",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExperienciasProfissionais_Perfil_PerfilId",
                table: "ExperienciasProfissionais",
                column: "PerfilId",
                principalTable: "Perfil",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Perfil_Users_OwnerId",
                table: "Perfil",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PerfilSkills_Perfil_PerfilId",
                table: "PerfilSkills",
                column: "PerfilId",
                principalTable: "Perfil",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TalentosElegiveis_Perfil_PerfilId",
                table: "TalentosElegiveis",
                column: "PerfilId",
                principalTable: "Perfil",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
