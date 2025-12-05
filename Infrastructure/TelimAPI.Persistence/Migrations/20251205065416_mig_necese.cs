using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelimAPI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_necese : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Trainings",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Trainings",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Trainings");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Trainings",
                newName: "Date");
        }
    }
}
