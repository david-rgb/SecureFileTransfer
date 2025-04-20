using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureFileSender.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailSettingsForAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "EmailSettings");

            migrationBuilder.RenameColumn(
                name: "SmtpUsername",
                table: "EmailSettings",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "SmtpPort",
                table: "EmailSettings",
                newName: "UseSSL");

            migrationBuilder.RenameColumn(
                name: "SmtpPassword",
                table: "EmailSettings",
                newName: "SmtpServer");

            migrationBuilder.RenameColumn(
                name: "SmtpHost",
                table: "EmailSettings",
                newName: "SenderEmail");

            migrationBuilder.RenameColumn(
                name: "FromEmail",
                table: "EmailSettings",
                newName: "SenderDisplayName");

            migrationBuilder.RenameColumn(
                name: "EnableSsl",
                table: "EmailSettings",
                newName: "Port");

            migrationBuilder.AddColumn<int>(
                name: "AdminUserId",
                table: "EmailSettings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "EmailSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EmailSettings_AdminUserId",
                table: "EmailSettings",
                column: "AdminUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailSettings_AdminUsers_AdminUserId",
                table: "EmailSettings",
                column: "AdminUserId",
                principalTable: "AdminUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailSettings_AdminUsers_AdminUserId",
                table: "EmailSettings");

            migrationBuilder.DropIndex(
                name: "IX_EmailSettings_AdminUserId",
                table: "EmailSettings");

            migrationBuilder.DropColumn(
                name: "AdminUserId",
                table: "EmailSettings");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "EmailSettings");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "EmailSettings",
                newName: "SmtpUsername");

            migrationBuilder.RenameColumn(
                name: "UseSSL",
                table: "EmailSettings",
                newName: "SmtpPort");

            migrationBuilder.RenameColumn(
                name: "SmtpServer",
                table: "EmailSettings",
                newName: "SmtpPassword");

            migrationBuilder.RenameColumn(
                name: "SenderEmail",
                table: "EmailSettings",
                newName: "SmtpHost");

            migrationBuilder.RenameColumn(
                name: "SenderDisplayName",
                table: "EmailSettings",
                newName: "FromEmail");

            migrationBuilder.RenameColumn(
                name: "Port",
                table: "EmailSettings",
                newName: "EnableSsl");

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "EmailSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
