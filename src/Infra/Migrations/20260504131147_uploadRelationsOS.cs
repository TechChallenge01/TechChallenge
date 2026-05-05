using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class uploadRelationsOS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicoPecas_OrdemServico_PecaId",
                table: "OrdemServicoPecas");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicoServicos_OrdemServico_ServicoId",
                table: "OrdemServicoServicos");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicoPecas_OrdemServico_OrdemServicoId",
                table: "OrdemServicoPecas",
                column: "OrdemServicoId",
                principalTable: "OrdemServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicoServicos_OrdemServico_OrdemServicoId",
                table: "OrdemServicoServicos",
                column: "OrdemServicoId",
                principalTable: "OrdemServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicoPecas_OrdemServico_OrdemServicoId",
                table: "OrdemServicoPecas");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicoServicos_OrdemServico_OrdemServicoId",
                table: "OrdemServicoServicos");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicoPecas_OrdemServico_PecaId",
                table: "OrdemServicoPecas",
                column: "PecaId",
                principalTable: "OrdemServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicoServicos_OrdemServico_ServicoId",
                table: "OrdemServicoServicos",
                column: "ServicoId",
                principalTable: "OrdemServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
