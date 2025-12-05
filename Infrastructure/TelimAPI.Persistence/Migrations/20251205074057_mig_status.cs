using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelimAPI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Trainings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Trainings");
        }
    }
}
