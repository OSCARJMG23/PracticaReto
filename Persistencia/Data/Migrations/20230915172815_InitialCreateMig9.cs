using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistencia.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateMig9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRols_User_UsuarioId",
                table: "UserRols");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRols_rol_RolId",
                table: "UserRols");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRols",
                table: "UserRols");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserRols");

            migrationBuilder.RenameTable(
                name: "UserRols",
                newName: "userRoles");

            migrationBuilder.RenameIndex(
                name: "IX_UserRols_RolId",
                table: "userRoles",
                newName: "IX_userRoles_RolId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Revoked",
                table: "refreshToken",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_userRoles",
                table: "userRoles",
                columns: new[] { "UsuarioId", "RolId" });

            migrationBuilder.AddForeignKey(
                name: "FK_userRoles_User_UsuarioId",
                table: "userRoles",
                column: "UsuarioId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userRoles_rol_RolId",
                table: "userRoles",
                column: "RolId",
                principalTable: "rol",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userRoles_User_UsuarioId",
                table: "userRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_userRoles_rol_RolId",
                table: "userRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userRoles",
                table: "userRoles");

            migrationBuilder.RenameTable(
                name: "userRoles",
                newName: "UserRols");

            migrationBuilder.RenameIndex(
                name: "IX_userRoles_RolId",
                table: "UserRols",
                newName: "IX_UserRols_RolId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Revoked",
                table: "refreshToken",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserRols",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRols",
                table: "UserRols",
                columns: new[] { "UsuarioId", "RolId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserRols_User_UsuarioId",
                table: "UserRols",
                column: "UsuarioId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRols_rol_RolId",
                table: "UserRols",
                column: "RolId",
                principalTable: "rol",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
