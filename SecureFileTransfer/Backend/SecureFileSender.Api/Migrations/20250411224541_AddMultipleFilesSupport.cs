using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureFileSender.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleFilesSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SharedFileLinkId",
                table: "UploadedFiles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_SharedFileLinkId",
                table: "UploadedFiles",
                column: "SharedFileLinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_SharedFileLinks_SharedFileLinkId",
                table: "UploadedFiles",
                column: "SharedFileLinkId",
                principalTable: "SharedFileLinks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_SharedFileLinks_SharedFileLinkId",
                table: "UploadedFiles");

            migrationBuilder.DropIndex(
                name: "IX_UploadedFiles_SharedFileLinkId",
                table: "UploadedFiles");

            migrationBuilder.DropColumn(
                name: "SharedFileLinkId",
                table: "UploadedFiles");
        }
    }
}
