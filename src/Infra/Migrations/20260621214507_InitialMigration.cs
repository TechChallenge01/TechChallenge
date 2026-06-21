using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Cnpj = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DDD = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    DDI = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    NumeroTelefone = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Logradouro = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Bairro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Uf = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    IdUsuarioCriacao = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuarioAtualizacao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Insumos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustoUnitario = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    IdUsuarioCriacao = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuarioAtualizacao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insumos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pecas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MarcaPeca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IdUsuarioCriacao = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuarioAtualizacao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pecas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TempoMedioExecucao = table.Column<TimeSpan>(type: "time", nullable: true),
                    IdUsuarioCriacao = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuarioAtualizacao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Perfil = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdUsuarioCriacao = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuarioAtualizacao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Veiculos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MarcaVeiculo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    Placa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Cor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UsuarioCriacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuarioAtualizacao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veiculos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veiculos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Estoques",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PecaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InsumoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QuantidadeDisponivel = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    QuantidadeReservada = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estoques", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estoques_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Estoques_Pecas_PecaId",
                        column: x => x.PecaId,
                        principalTable: "Pecas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrdemServico",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VeiculoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusOS = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    ValorDesconto = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    InicioExecucao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TerminoExecucao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdUsuarioCriacao = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuarioAtualizacao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdemServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdemServico_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdemServico_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EstoqueHistoricos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TipoMovimentacao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EstoqueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUsuarioCriacao = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstoqueHistoricos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstoqueHistoricos_Estoques_EstoqueId",
                        column: x => x.EstoqueId,
                        principalTable: "Estoques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdemServicoInsumos",
                columns: table => new
                {
                    InsumoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrdemServicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    CustoUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdemServicoInsumos", x => new { x.OrdemServicoId, x.InsumoId });
                    table.ForeignKey(
                        name: "FK_OrdemServicoInsumos_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdemServicoInsumos_OrdemServico_OrdemServicoId",
                        column: x => x.OrdemServicoId,
                        principalTable: "OrdemServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdemServicoPecas",
                columns: table => new
                {
                    OrdemServicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PecaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdemServicoPecas", x => new { x.OrdemServicoId, x.PecaId });
                    table.ForeignKey(
                        name: "FK_OrdemServicoPecas_OrdemServico_OrdemServicoId",
                        column: x => x.OrdemServicoId,
                        principalTable: "OrdemServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdemServicoPecas_Pecas_PecaId",
                        column: x => x.PecaId,
                        principalTable: "Pecas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdemServicoServicos",
                columns: table => new
                {
                    OrdemServicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DataInicioExecucao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataTerminoExecucao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quantidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdemServicoServicos", x => new { x.OrdemServicoId, x.ServicoId });
                    table.ForeignKey(
                        name: "FK_OrdemServicoServicos_OrdemServico_OrdemServicoId",
                        column: x => x.OrdemServicoId,
                        principalTable: "OrdemServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdemServicoServicos_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstoqueHistoricos_EstoqueId",
                table: "EstoqueHistoricos",
                column: "EstoqueId");

            migrationBuilder.CreateIndex(
                name: "IX_Estoques_InsumoId",
                table: "Estoques",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_Estoques_PecaId",
                table: "Estoques",
                column: "PecaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServico_ClienteId",
                table: "OrdemServico",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServico_VeiculoId",
                table: "OrdemServico",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServicoInsumos_InsumoId",
                table: "OrdemServicoInsumos",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServicoPecas_PecaId",
                table: "OrdemServicoPecas",
                column: "PecaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServicoServicos_ServicoId",
                table: "OrdemServicoServicos",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_ClienteId",
                table: "Veiculos",
                column: "ClienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstoqueHistoricos");

            migrationBuilder.DropTable(
                name: "OrdemServicoInsumos");

            migrationBuilder.DropTable(
                name: "OrdemServicoPecas");

            migrationBuilder.DropTable(
                name: "OrdemServicoServicos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Estoques");

            migrationBuilder.DropTable(
                name: "OrdemServico");

            migrationBuilder.DropTable(
                name: "Servicos");

            migrationBuilder.DropTable(
                name: "Insumos");

            migrationBuilder.DropTable(
                name: "Pecas");

            migrationBuilder.DropTable(
                name: "Veiculos");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
