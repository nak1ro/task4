using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLastLoginTimeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_LastLoginTime",
                table: "Users",
                column: "LastLoginTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_LastLoginTime",
                table: "Users");
        }
    }
}
