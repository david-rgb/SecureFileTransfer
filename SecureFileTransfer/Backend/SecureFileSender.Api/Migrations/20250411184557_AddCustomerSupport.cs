using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureFileSender.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Used",
                table: "SharedFileLinks",
                newName: "CustomerId");

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SharedFileLinks_CustomerId",
                table: "SharedFileLinks",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedFileLinks_Customer_CustomerId",
                table: "SharedFileLinks",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedFileLinks_Customer_CustomerId",
                table: "SharedFileLinks");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_SharedFileLinks_CustomerId",
                table: "SharedFileLinks");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "SharedFileLinks",
                newName: "Used");
        }
    }
}
