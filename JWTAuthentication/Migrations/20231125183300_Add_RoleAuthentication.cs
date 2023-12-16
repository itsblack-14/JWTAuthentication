using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTAuthentication.Migrations
{
    public partial class Add_RoleAuthentication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoleAuthentication",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    AuthenticationSettingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleAuthentication", x => new { x.RoleId, x.AuthenticationSettingId });
                    table.ForeignKey(
                        name: "FK_RoleAuthentication_AuthenticationSetting_AuthenticationSettingId",
                        column: x => x.AuthenticationSettingId,
                        principalTable: "AuthenticationSetting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleAuthentication_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleAuthentication_AuthenticationSettingId",
                table: "RoleAuthentication",
                column: "AuthenticationSettingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleAuthentication");
        }
    }
}
