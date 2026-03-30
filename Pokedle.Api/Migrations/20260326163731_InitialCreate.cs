using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pokedle.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "colors",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_colors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "element_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_element_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "habitats",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_habitats", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pokemons",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    generation = table.Column<int>(type: "integer", nullable: false),
                    evolution_stage = table.Column<int>(type: "integer", nullable: false),
                    habitat_id = table.Column<int>(type: "integer", nullable: false),
                    color_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pokemons", x => x.id);
                    table.ForeignKey(
                        name: "fk_pokemons_colors_color_id",
                        column: x => x.color_id,
                        principalTable: "colors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pokemons_habitats_habitat_id",
                        column: x => x.habitat_id,
                        principalTable: "habitats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pokemon_element_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pokemon_id = table.Column<int>(type: "integer", nullable: false),
                    element_type_id = table.Column<int>(type: "integer", nullable: false),
                    slot = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pokemon_element_types", x => x.id);
                    table.ForeignKey(
                        name: "fk_pokemon_element_types_element_types_element_type_id",
                        column: x => x.element_type_id,
                        principalTable: "element_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pokemon_element_types_pokemons_pokemon_id",
                        column: x => x.pokemon_id,
                        principalTable: "pokemons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pokemon_element_types_element_type_id",
                table: "pokemon_element_types",
                column: "element_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_pokemon_element_types_pokemon_id_slot",
                table: "pokemon_element_types",
                columns: new[] { "pokemon_id", "slot" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pokemons_color_id",
                table: "pokemons",
                column: "color_id");

            migrationBuilder.CreateIndex(
                name: "ix_pokemons_habitat_id",
                table: "pokemons",
                column: "habitat_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pokemon_element_types");

            migrationBuilder.DropTable(
                name: "element_types");

            migrationBuilder.DropTable(
                name: "pokemons");

            migrationBuilder.DropTable(
                name: "colors");

            migrationBuilder.DropTable(
                name: "habitats");
        }
    }
}
