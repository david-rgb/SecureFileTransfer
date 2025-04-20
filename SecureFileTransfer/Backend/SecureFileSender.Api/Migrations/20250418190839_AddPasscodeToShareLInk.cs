using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureFileSender.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPasscodeToShareLInk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PasscodeHash",
                table: "SharedFileLinks",
                newName: "Passcode");

            migrationBuilder.AddColumn<bool>(
                name: "RequiresPasscode",
                table: "SharedFileLinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiresPasscode",
                table: "SharedFileLinks");

            migrationBuilder.RenameColumn(
                name: "Passcode",
                table: "SharedFileLinks",
                newName: "PasscodeHash");
        }
    }
}
