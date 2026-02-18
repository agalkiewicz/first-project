using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace first_project.Migrations
{
    /// <inheritdoc />
    public partial class AddPaginationIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Movies_Created",
                schema: "app",
                table: "Movies",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Genre",
                schema: "app",
                table: "Movies",
                column: "Genre");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Rating",
                schema: "app",
                table: "Movies",
                column: "Rating");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Movies_Created",
                schema: "app",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Genre",
                schema: "app",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Rating",
                schema: "app",
                table: "Movies");
        }
    }
}
