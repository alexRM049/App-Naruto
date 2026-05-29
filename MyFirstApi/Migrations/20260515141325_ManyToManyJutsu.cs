using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFirstApi.Migrations
{
    /// <inheritdoc />
    public partial class ManyToManyJutsu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Shinobis",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Shinobis");
        }
    }
}
