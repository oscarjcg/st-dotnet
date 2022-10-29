using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace st_dotnet.Migrations
{
    public partial class add_channeltype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "channels");

            migrationBuilder.AddColumn<int>(
                name: "ChannelTypeId",
                table: "channels",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelTypeId",
                table: "channels");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "channels",
                type: "int",
                maxLength: 300,
                nullable: false,
                defaultValue: 0);
        }
    }
}
