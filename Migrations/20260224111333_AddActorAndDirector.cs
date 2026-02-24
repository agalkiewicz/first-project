using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace first_project.Migrations
{
    /// <inheritdoc />
    public partial class AddActorAndDirector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DirectorId",
                schema: "app",
                table: "Movies",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Actors",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Directors",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovieActors",
                schema: "app",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "integer", nullable: false),
                    ActorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieActors", x => new { x.MovieId, x.ActorId });
                    table.ForeignKey(
                        name: "FK_MovieActors_Actors_ActorId",
                        column: x => x.ActorId,
                        principalSchema: "app",
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieActors_Movies_MovieId",
                        column: x => x.MovieId,
                        principalSchema: "app",
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_DirectorId",
                schema: "app",
                table: "Movies",
                column: "DirectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Actors_FirstName_LastName",
                schema: "app",
                table: "Actors",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Directors_FirstName_LastName",
                schema: "app",
                table: "Directors",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_MovieActors_ActorId",
                schema: "app",
                table: "MovieActors",
                column: "ActorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Directors_DirectorId",
                schema: "app",
                table: "Movies",
                column: "DirectorId",
                principalSchema: "app",
                principalTable: "Directors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Directors_DirectorId",
                schema: "app",
                table: "Movies");

            migrationBuilder.DropTable(
                name: "Directors",
                schema: "app");

            migrationBuilder.DropTable(
                name: "MovieActors",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Actors",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_Movies_DirectorId",
                schema: "app",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "DirectorId",
                schema: "app",
                table: "Movies");
        }
    }
}
