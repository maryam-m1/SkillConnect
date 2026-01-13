using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillConnect.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Code removed because the tables already exist in your SQL Server.
            // Emptying this method prevents the "There is already an object named 'Users'" error.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Emptying this to match the Up method.
        }
    }
}