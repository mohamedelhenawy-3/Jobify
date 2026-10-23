using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addingpimageforprojects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_JobSeekers_JobSeekerProfileId",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Projects",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Projects");

            migrationBuilder.RenameTable(
                name: "Projects",
                newName: "SeekerProjects");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_JobSeekerProfileId",
                table: "SeekerProjects",
                newName: "IX_SeekerProjects_JobSeekerProfileId");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "ProfileImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCover",
                table: "ProfileImages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SeekerProjectId",
                table: "ProfileImages",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "SeekerProjects",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SeekerProjects",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Technologies",
                table: "SeekerProjects",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SeekerProjects",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SeekerProjects",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "SeekerProjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SeekerProjects",
                table: "SeekerProjects",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileImages_SeekerProjectId_DisplayOrder",
                table: "ProfileImages",
                columns: new[] { "SeekerProjectId", "DisplayOrder" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileImages_SeekerProjects_SeekerProjectId",
                table: "ProfileImages",
                column: "SeekerProjectId",
                principalTable: "SeekerProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SeekerProjects_JobSeekers_JobSeekerProfileId",
                table: "SeekerProjects",
                column: "JobSeekerProfileId",
                principalTable: "JobSeekers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileImages_SeekerProjects_SeekerProjectId",
                table: "ProfileImages");

            migrationBuilder.DropForeignKey(
                name: "FK_SeekerProjects_JobSeekers_JobSeekerProfileId",
                table: "SeekerProjects");

            migrationBuilder.DropIndex(
                name: "IX_ProfileImages_SeekerProjectId_DisplayOrder",
                table: "ProfileImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SeekerProjects",
                table: "SeekerProjects");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "ProfileImages");

            migrationBuilder.DropColumn(
                name: "IsCover",
                table: "ProfileImages");

            migrationBuilder.DropColumn(
                name: "SeekerProjectId",
                table: "ProfileImages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SeekerProjects");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "SeekerProjects");

            migrationBuilder.RenameTable(
                name: "SeekerProjects",
                newName: "Projects");

            migrationBuilder.RenameIndex(
                name: "IX_SeekerProjects_JobSeekerProfileId",
                table: "Projects",
                newName: "IX_Projects_JobSeekerProfileId");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Technologies",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Projects",
                table: "Projects",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_JobSeekers_JobSeekerProfileId",
                table: "Projects",
                column: "JobSeekerProfileId",
                principalTable: "JobSeekers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
