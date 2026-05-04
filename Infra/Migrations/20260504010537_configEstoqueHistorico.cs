using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class configEstoqueHistorico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EstoqueHistoricos_Estoques_EstoqueId1",
                table: "EstoqueHistoricos");

            migrationBuilder.DropIndex(
                name: "IX_EstoqueHistoricos_EstoqueId1",
                table: "EstoqueHistoricos");

            migrationBuilder.DropColumn(
                name: "EstoqueId1",
                table: "EstoqueHistoricos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EstoqueId1",
                table: "EstoqueHistoricos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstoqueHistoricos_EstoqueId1",
                table: "EstoqueHistoricos",
                column: "EstoqueId1",
                unique: true,
                filter: "[EstoqueId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_EstoqueHistoricos_Estoques_EstoqueId1",
                table: "EstoqueHistoricos",
                column: "EstoqueId1",
                principalTable: "Estoques",
                principalColumn: "Id");
        }
    }
}
