using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addinglangsertificate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeekerCertificate");

            migrationBuilder.DropTable(
                name: "SeekerLanguage");

            migrationBuilder.DropTable(
                name: "SeekerProject");

            migrationBuilder.AddColumn<int>(
                name: "JobSeekerProfileId",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "JobSeekerProfileId",
                table: "Languages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "JobSeekerProfileId",
                table: "Certificates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_JobSeekerProfileId",
                table: "Projects",
                column: "JobSeekerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_JobSeekerProfileId",
                table: "Languages",
                column: "JobSeekerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_JobSeekerProfileId",
                table: "Certificates",
                column: "JobSeekerProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_JobSeekers_JobSeekerProfileId",
                table: "Certificates",
                column: "JobSeekerProfileId",
                principalTable: "JobSeekers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_JobSeekers_JobSeekerProfileId",
                table: "Languages",
                column: "JobSeekerProfileId",
                principalTable: "JobSeekers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_JobSeekers_JobSeekerProfileId",
                table: "Projects",
                column: "JobSeekerProfileId",
                principalTable: "JobSeekers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_JobSeekers_JobSeekerProfileId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Languages_JobSeekers_JobSeekerProfileId",
                table: "Languages");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_JobSeekers_JobSeekerProfileId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_JobSeekerProfileId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Languages_JobSeekerProfileId",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_JobSeekerProfileId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "JobSeekerProfileId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "JobSeekerProfileId",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "JobSeekerProfileId",
                table: "Certificates");

            migrationBuilder.CreateTable(
                name: "SeekerCertificate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CredentialUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Issuer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobSeekerProfileId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeekerCertificate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeekerCertificate_JobSeekers_JobSeekerProfileId",
                        column: x => x.JobSeekerProfileId,
                        principalTable: "JobSeekers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeekerLanguage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobSeekerProfileId = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeekerLanguage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeekerLanguage_JobSeekers_JobSeekerProfileId",
                        column: x => x.JobSeekerProfileId,
                        principalTable: "JobSeekers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeekerProject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JobSeekerProfileId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Technologies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeekerProject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeekerProject_JobSeekers_JobSeekerProfileId",
                        column: x => x.JobSeekerProfileId,
                        principalTable: "JobSeekers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeekerCertificate_JobSeekerProfileId",
                table: "SeekerCertificate",
                column: "JobSeekerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SeekerLanguage_JobSeekerProfileId",
                table: "SeekerLanguage",
                column: "JobSeekerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SeekerProject_JobSeekerProfileId",
                table: "SeekerProject",
                column: "JobSeekerProfileId");
        }
    }
}
