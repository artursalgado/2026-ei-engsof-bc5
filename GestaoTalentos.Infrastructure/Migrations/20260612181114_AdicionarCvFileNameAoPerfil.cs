using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoTalentos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCvFileNameAoPerfil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CvFileName",
                table: "Perfis",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CvFileName",
                table: "Perfis");
        }
    }
}
