using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updateseekrprofile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educations_JobSeekers_JobSeekerId",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_JobSeekers_JobSeekerId",
                table: "Experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_JobSeekers_JobSeekerId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_JobSeekerId",
                table: "Skills");

            migrationBuilder.RenameColumn(
                name: "JobSeekerId",
                table: "Skills",
                newName: "SeekerProfileId");

            migrationBuilder.RenameColumn(
                name: "JobTitle",
                table: "Experiences",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "JobSeekerId",
                table: "Experiences",
                newName: "SeekerProfileId");

            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "Experiences",
                newName: "Company");

            migrationBuilder.RenameIndex(
                name: "IX_Experiences_JobSeekerId",
                table: "Experiences",
                newName: "IX_Experiences_SeekerProfileId");

            migrationBuilder.RenameColumn(
                name: "JobSeekerId",
                table: "Educations",
                newName: "SeekerProfileId");

            migrationBuilder.RenameColumn(
                name: "Institution",
                table: "Educations",
                newName: "School");

            migrationBuilder.RenameIndex(
                name: "IX_Educations_JobSeekerId",
                table: "Educations",
                newName: "IX_Educations_SeekerProfileId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Skills",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "DesiredRole",
                table: "JobSeekers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpectedSalary",
                table: "JobSeekers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOpenToWork",
                table: "JobSeekers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "JobSeekers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NoticePeriod",
                table: "JobSeekers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredJobType",
                table: "JobSeekers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredLocations",
                table: "JobSeekers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowCv",
                table: "JobSeekers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowEmail",
                table: "JobSeekers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowPhone",
                table: "JobSeekers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "JobSeekers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "JobSeekers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCurrent",
                table: "Experiences",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Experiences",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Educations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skills_SeekerProfileId_Name",
                table: "Skills",
                columns: new[] { "SeekerProfileId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekers_Username",
                table: "JobSeekers",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_JobSeekers_SeekerProfileId",
                table: "Educations",
                column: "SeekerProfileId",
                principalTable: "JobSeekers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_JobSeekers_SeekerProfileId",
                table: "Experiences",
                column: "SeekerProfileId",
                principalTable: "JobSeekers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_JobSeekers_SeekerProfileId",
                table: "Skills",
                column: "SeekerProfileId",
                principalTable: "JobSeekers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educations_JobSeekers_SeekerProfileId",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_JobSeekers_SeekerProfileId",
                table: "Experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_JobSeekers_SeekerProfileId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_SeekerProfileId_Name",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_JobSeekers_Username",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "DesiredRole",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "ExpectedSalary",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "IsOpenToWork",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "NoticePeriod",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "PreferredJobType",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "PreferredLocations",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "ShowCv",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "ShowEmail",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "ShowPhone",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "IsCurrent",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Educations");

            migrationBuilder.RenameColumn(
                name: "SeekerProfileId",
                table: "Skills",
                newName: "JobSeekerId");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Experiences",
                newName: "JobTitle");

            migrationBuilder.RenameColumn(
                name: "SeekerProfileId",
                table: "Experiences",
                newName: "JobSeekerId");

            migrationBuilder.RenameColumn(
                name: "Company",
                table: "Experiences",
                newName: "CompanyName");

            migrationBuilder.RenameIndex(
                name: "IX_Experiences_SeekerProfileId",
                table: "Experiences",
                newName: "IX_Experiences_JobSeekerId");

            migrationBuilder.RenameColumn(
                name: "SeekerProfileId",
                table: "Educations",
                newName: "JobSeekerId");

            migrationBuilder.RenameColumn(
                name: "School",
                table: "Educations",
                newName: "Institution");

            migrationBuilder.RenameIndex(
                name: "IX_Educations_SeekerProfileId",
                table: "Educations",
                newName: "IX_Educations_JobSeekerId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Skills",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_JobSeekerId",
                table: "Skills",
                column: "JobSeekerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_JobSeekers_JobSeekerId",
                table: "Educations",
                column: "JobSeekerId",
                principalTable: "JobSeekers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_JobSeekers_JobSeekerId",
                table: "Experiences",
                column: "JobSeekerId",
                principalTable: "JobSeekers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_JobSeekers_JobSeekerId",
                table: "Skills",
                column: "JobSeekerId",
                principalTable: "JobSeekers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
