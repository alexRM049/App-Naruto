using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFirstApi.Migrations
{
    /// <inheritdoc />
    public partial class FixJutsuRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jutsus_Shinobis_ShinobiId",
                table: "Jutsus");

            migrationBuilder.DropIndex(
                name: "IX_Jutsus_ShinobiId",
                table: "Jutsus");

            migrationBuilder.DropColumn(
                name: "ShinobiId",
                table: "Jutsus");

            migrationBuilder.CreateTable(
                name: "JutsuShinobi",
                columns: table => new
                {
                    JutsusId = table.Column<int>(type: "int", nullable: false),
                    ShinobisId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JutsuShinobi", x => new { x.JutsusId, x.ShinobisId });
                    table.ForeignKey(
                        name: "FK_JutsuShinobi_Jutsus_JutsusId",
                        column: x => x.JutsusId,
                        principalTable: "Jutsus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JutsuShinobi_Shinobis_ShinobisId",
                        column: x => x.ShinobisId,
                        principalTable: "Shinobis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JutsuShinobi_ShinobisId",
                table: "JutsuShinobi",
                column: "ShinobisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JutsuShinobi");

            migrationBuilder.AddColumn<int>(
                name: "ShinobiId",
                table: "Jutsus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Jutsus_ShinobiId",
                table: "Jutsus",
                column: "ShinobiId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jutsus_Shinobis_ShinobiId",
                table: "Jutsus",
                column: "ShinobiId",
                principalTable: "Shinobis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
