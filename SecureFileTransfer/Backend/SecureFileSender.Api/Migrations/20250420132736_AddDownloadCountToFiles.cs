﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureFileSender.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDownloadCountToFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DownloadCount",
                table: "UploadedFiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownloadCount",
                table: "UploadedFiles");
        }
    }
}
