using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagementSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class changeuserRoleUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userRoles_Roles_RoleId",
                table: "userRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_userRoles_Users_UserId",
                table: "userRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userRoles",
                table: "userRoles");

            migrationBuilder.RenameTable(
                name: "userRoles",
                newName: "UserRoles");

            migrationBuilder.RenameIndex(
                name: "IX_userRoles_RoleId",
                table: "UserRoles",
                newName: "IX_UserRoles_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "userRoles");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_RoleId",
                table: "userRoles",
                newName: "IX_userRoles_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_userRoles",
                table: "userRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_userRoles_Roles_RoleId",
                table: "userRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userRoles_Users_UserId",
                table: "userRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
