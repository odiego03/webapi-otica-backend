using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi_otica.Migrations
{
    /// <inheritdoc />
    public partial class slugAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Colecoes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Colecoes");
        }
    }
}
