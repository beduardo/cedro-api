using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace api.Migrations
{
    public partial class criacaomodelo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Restaurantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DataCriacao = table.Column<DateTimeOffset>(nullable: false),
                    DataAlteracao = table.Column<DateTimeOffset>(nullable: false),
                    Excluido = table.Column<bool>(nullable: false),
                    Nome = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pratos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DataCriacao = table.Column<DateTimeOffset>(nullable: false),
                    DataAlteracao = table.Column<DateTimeOffset>(nullable: false),
                    Excluido = table.Column<bool>(nullable: false),
                    RestauranteId = table.Column<Guid>(nullable: false),
                    Nome = table.Column<string>(maxLength: 100, nullable: false),
                    Preco = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pratos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pratos_Restaurantes_RestauranteId",
                        column: x => x.RestauranteId,
                        principalTable: "Restaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pratos_RestauranteId",
                table: "Pratos",
                column: "RestauranteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pratos");

            migrationBuilder.DropTable(
                name: "Restaurantes");
        }
    }
}
