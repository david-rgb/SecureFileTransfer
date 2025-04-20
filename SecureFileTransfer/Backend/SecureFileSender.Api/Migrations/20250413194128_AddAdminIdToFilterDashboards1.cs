using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureFileSender.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminIdToFilterDashboards1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdminUserId",
                table: "Customers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AdminUserId",
                table: "Customers",
                column: "AdminUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AdminUsers_AdminUserId",
                table: "Customers",
                column: "AdminUserId",
                principalTable: "AdminUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AdminUsers_AdminUserId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_AdminUserId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AdminUserId",
                table: "Customers");
        }
    }
}
