using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureFileSender.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminIdToFilterDashboards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdminUserId",
                table: "UploadedFiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdminUserId",
                table: "SharedFileLinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_AdminUserId",
                table: "UploadedFiles",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedFileLinks_AdminUserId",
                table: "SharedFileLinks",
                column: "AdminUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedFileLinks_AdminUsers_AdminUserId",
                table: "SharedFileLinks",
                column: "AdminUserId",
                principalTable: "AdminUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_AdminUsers_AdminUserId",
                table: "UploadedFiles",
                column: "AdminUserId",
                principalTable: "AdminUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedFileLinks_AdminUsers_AdminUserId",
                table: "SharedFileLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_AdminUsers_AdminUserId",
                table: "UploadedFiles");

            migrationBuilder.DropIndex(
                name: "IX_UploadedFiles_AdminUserId",
                table: "UploadedFiles");

            migrationBuilder.DropIndex(
                name: "IX_SharedFileLinks_AdminUserId",
                table: "SharedFileLinks");

            migrationBuilder.DropColumn(
                name: "AdminUserId",
                table: "UploadedFiles");

            migrationBuilder.DropColumn(
                name: "AdminUserId",
                table: "SharedFileLinks");
        }
    }
}
