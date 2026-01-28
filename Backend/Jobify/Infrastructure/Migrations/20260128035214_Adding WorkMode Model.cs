using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingWorkModeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AddColumn<int>(
                name: "WorkModeId",
                table: "JobPosts",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "WorkModes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkModes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "WorkModes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "OnSite" },
                    { 2, "Remote" },
                    { 3, "Hybrid" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_WorkModeId",
                table: "JobPosts",
                column: "WorkModeId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPosts_WorkModes_WorkModeId",
                table: "JobPosts",
                column: "WorkModeId",
                principalTable: "WorkModes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPosts_WorkModes_WorkModeId",
                table: "JobPosts");

            migrationBuilder.DropTable(
                name: "WorkModes");

            migrationBuilder.DropIndex(
                name: "IX_JobPosts_WorkModeId",
                table: "JobPosts");

            migrationBuilder.DropColumn(
                name: "WorkModeId",
                table: "JobPosts");

            migrationBuilder.InsertData(
                table: "EmploymentTypes",
                columns: new[] { "Id", "TypeName" },
                values: new object[] { 5, "Remote" });
        }
    }
}
