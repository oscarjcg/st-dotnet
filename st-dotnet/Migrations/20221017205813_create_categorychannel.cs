using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace st_dotnet.Migrations
{
    public partial class create_categorychannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryChannel",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    ChannelsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryChannel", x => new { x.CategoriesId, x.ChannelsId });
                    table.ForeignKey(
                        name: "FK_CategoryChannel_categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryChannel_channels_ChannelsId",
                        column: x => x.ChannelsId,
                        principalTable: "channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryChannel_ChannelsId",
                table: "CategoryChannel",
                column: "ChannelsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryChannel");
        }
    }
}
