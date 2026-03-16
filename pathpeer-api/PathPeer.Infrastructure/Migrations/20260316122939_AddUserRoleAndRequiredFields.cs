using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PathPeer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoleAndRequiredFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Pasul 1 — Adaugi coloana nouă integer
            migrationBuilder.AddColumn<int>(
                name: "Level_New",
                table: "Courses",
                type: "integer",
                nullable: true);

            // Pasul 2 — Migrezi datele string → integer
            migrationBuilder.Sql(@"
                UPDATE ""Courses"" SET ""Level_New"" = CASE
                    WHEN ""Level"" = 'Beginner'     THEN 0
                    WHEN ""Level"" = 'Intermediate' THEN 1
                    WHEN ""Level"" = 'Advanced'     THEN 2
                    ELSE NULL
                END
            ");

            // Pasul 3 — Ștergi coloana veche
            migrationBuilder.DropColumn(
                name: "Level",
                table: "Courses");

            // Pasul 4 — Redenumești
            migrationBuilder.RenameColumn(
                name: "Level_New",
                table: "Courses",
                newName: "Level");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "Courses",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
