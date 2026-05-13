using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PathPeer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseVote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewEndsAt",
                table: "Courses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewStartedAt",
                table: "Courses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VotesDown",
                table: "Courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VotesUp",
                table: "Courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CourseVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseVotes_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseVotes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseVotes_CourseId",
                table: "CourseVotes",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseVotes_UserId_CourseId",
                table: "CourseVotes",
                columns: new[] { "UserId", "CourseId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseVotes");

            migrationBuilder.DropColumn(
                name: "ReviewEndsAt",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ReviewStartedAt",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "VotesDown",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "VotesUp",
                table: "Courses");
        }
    }
}
