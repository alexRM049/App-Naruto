using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFirstApi.Migrations
{
    /// <inheritdoc />
    public partial class RelationshipSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Clan",
                table: "Shinobis");

            migrationBuilder.DropColumn(
                name: "IsJinchuriki",
                table: "Shinobis");

            migrationBuilder.DropColumn(
                name: "Village",
                table: "Shinobis");

            migrationBuilder.AddColumn<int>(
                name: "VillageId",
                table: "Shinobis",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Jutsus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Element = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShinobiId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jutsus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jutsus_Shinobis_ShinobiId",
                        column: x => x.ShinobiId,
                        principalTable: "Shinobis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Villages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KageTitle = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Villages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shinobis_VillageId",
                table: "Shinobis",
                column: "VillageId");

            migrationBuilder.CreateIndex(
                name: "IX_Jutsus_ShinobiId",
                table: "Jutsus",
                column: "ShinobiId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shinobis_Villages_VillageId",
                table: "Shinobis",
                column: "VillageId",
                principalTable: "Villages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shinobis_Villages_VillageId",
                table: "Shinobis");

            migrationBuilder.DropTable(
                name: "Jutsus");

            migrationBuilder.DropTable(
                name: "Villages");

            migrationBuilder.DropIndex(
                name: "IX_Shinobis_VillageId",
                table: "Shinobis");

            migrationBuilder.DropColumn(
                name: "VillageId",
                table: "Shinobis");

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
                name: "Village",
                table: "Shinobis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
