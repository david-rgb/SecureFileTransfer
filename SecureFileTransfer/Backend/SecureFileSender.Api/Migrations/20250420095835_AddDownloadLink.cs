using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureFileSender.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDownloadLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DownloadLinkId",
                table: "UploadedFiles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DownloadLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Token = table.Column<Guid>(type: "TEXT", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", nullable: false),
                    IsPasscodeProtected = table.Column<bool>(type: "INTEGER", nullable: false),
                    Passcode = table.Column<string>(type: "TEXT", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AdminUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DownloadLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DownloadLinks_AdminUsers_AdminUserId",
                        column: x => x.AdminUserId,
                        principalTable: "AdminUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DownloadLinks_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_DownloadLinkId",
                table: "UploadedFiles",
                column: "DownloadLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_DownloadLinks_AdminUserId",
                table: "DownloadLinks",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DownloadLinks_CustomerId",
                table: "DownloadLinks",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_DownloadLinks_DownloadLinkId",
                table: "UploadedFiles",
                column: "DownloadLinkId",
                principalTable: "DownloadLinks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_DownloadLinks_DownloadLinkId",
                table: "UploadedFiles");

            migrationBuilder.DropTable(
                name: "DownloadLinks");

            migrationBuilder.DropIndex(
                name: "IX_UploadedFiles_DownloadLinkId",
                table: "UploadedFiles");

            migrationBuilder.DropColumn(
                name: "DownloadLinkId",
                table: "UploadedFiles");
        }
    }
}
