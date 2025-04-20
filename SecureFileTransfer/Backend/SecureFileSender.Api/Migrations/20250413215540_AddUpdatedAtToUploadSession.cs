﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureFileSender.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedAtToUploadSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UploadSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UploadSessions");
        }
    }
}
