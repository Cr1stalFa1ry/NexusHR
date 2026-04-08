using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employees.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalEmployeeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "HireDate",
                table: "employees",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                table: "employees",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Salary",
                table: "employees",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "HireDate",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "employees");
        }
    }
}
