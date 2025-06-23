using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmesAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaCamposDetalhados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataLancamento",
                table: "Filmes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Diretor",
                table: "Filmes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DuracaoMinutos",
                table: "Filmes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImagemUrl",
                table: "Filmes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Nota",
                table: "Filmes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Sinopse",
                table: "Filmes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataLancamento",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "Diretor",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "DuracaoMinutos",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "ImagemUrl",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "Nota",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "Sinopse",
                table: "Filmes");
        }
    }
}
