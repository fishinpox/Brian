using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calendar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFolders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "PersonalEvents",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SubfolderId",
                table: "PersonalEvents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CalendarSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MasterLockEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockFoldersByDefault = table.Column<bool>(type: "bit", nullable: false),
                    AutoRecolorByTimeSensitivity = table.Column<bool>(type: "bit", nullable: false),
                    DueSoonWindowHours = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ColorBackground = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ColorText = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ColorBorder = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subfolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FolderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subfolders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonalEvents_SubfolderId",
                table: "PersonalEvents",
                column: "SubfolderId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSettings_ProfileId",
                table: "CalendarSettings",
                column: "ProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Folders_ProfileId",
                table: "Folders",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Subfolders_FolderId",
                table: "Subfolders",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Subfolders_ProfileId",
                table: "Subfolders",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalEvents_Subfolders_SubfolderId",
                table: "PersonalEvents",
                column: "SubfolderId",
                principalTable: "Subfolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalEvents_Subfolders_SubfolderId",
                table: "PersonalEvents");

            migrationBuilder.DropTable(
                name: "CalendarSettings");

            migrationBuilder.DropTable(
                name: "Folders");

            migrationBuilder.DropTable(
                name: "Subfolders");

            migrationBuilder.DropIndex(
                name: "IX_PersonalEvents_SubfolderId",
                table: "PersonalEvents");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "PersonalEvents");

            migrationBuilder.DropColumn(
                name: "SubfolderId",
                table: "PersonalEvents");
        }
    }
}
