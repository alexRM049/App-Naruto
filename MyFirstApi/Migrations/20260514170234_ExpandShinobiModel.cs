using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFirstApi.Migrations
{
    /// <inheritdoc />
    public partial class ExpandShinobiModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Clan",
                table: "Shinobis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsJinchuriki",
                table: "Shinobis",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Rank",
                table: "Shinobis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Village",
                table: "Shinobis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Clan",
                table: "Shinobis");

            migrationBuilder.DropColumn(
                name: "IsJinchuriki",
                table: "Shinobis");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Shinobis");

            migrationBuilder.DropColumn(
                name: "Village",
                table: "Shinobis");
        }
    }
}
