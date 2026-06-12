using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Restaurant.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DishMEdiRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Dishes");

            migrationBuilder.AddColumn<long>(
                name: "MediaPicId",
                table: "Dishes",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DishMedias",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MediaType = table.Column<int>(type: "integer", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    FileWidth = table.Column<double>(type: "double precision", nullable: false),
                    FileHeight = table.Column<double>(type: "double precision", nullable: false),
                    FileSize = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishMedias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DishMediaRelations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DishId = table.Column<long>(type: "bigint", nullable: false),
                    MediaId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishMediaRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishMediaRelations_DishMedias_MediaId",
                        column: x => x.MediaId,
                        principalTable: "DishMedias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishMediaRelations_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_MediaPicId",
                table: "Dishes",
                column: "MediaPicId");

            migrationBuilder.CreateIndex(
                name: "IX_DishMediaRelations_DishId",
                table: "DishMediaRelations",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_DishMediaRelations_MediaId",
                table: "DishMediaRelations",
                column: "MediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_DishMedias_MediaPicId",
                table: "Dishes",
                column: "MediaPicId",
                principalTable: "DishMedias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_DishMedias_MediaPicId",
                table: "Dishes");

            migrationBuilder.DropTable(
                name: "DishMediaRelations");

            migrationBuilder.DropTable(
                name: "DishMedias");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_MediaPicId",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "MediaPicId",
                table: "Dishes");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Dishes",
                type: "text",
                nullable: true);
        }
    }
}
