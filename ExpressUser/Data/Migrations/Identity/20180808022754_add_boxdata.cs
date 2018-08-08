using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpressUser.Data.Migrations.Identity
{
    public partial class add_boxdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Express_Box",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    EId = table.Column<string>(nullable: true),
                    BoxArea = table.Column<string>(nullable: true),
                    BoxNum = table.Column<string>(nullable: true),
                    BoxStatus = table.Column<int>(nullable: false),
                    BoxCode = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Express_Box", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Express_Box");
        }
    }
}
