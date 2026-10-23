using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatingcompanyactivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EntityId",
                table: "CompanyActivities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityName",
                table: "CompanyActivities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EntityType",
                table: "CompanyActivities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MetaJson",
                table: "CompanyActivities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RefId",
                table: "CompanyActivities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyActivities_ActorEmployerId_CreatedAt",
                table: "CompanyActivities",
                columns: new[] { "ActorEmployerId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyActivities_TargetEmployerId_CreatedAt",
                table: "CompanyActivities",
                columns: new[] { "TargetEmployerId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyActivities_ActorEmployerId_CreatedAt",
                table: "CompanyActivities");

            migrationBuilder.DropIndex(
                name: "IX_CompanyActivities_TargetEmployerId_CreatedAt",
                table: "CompanyActivities");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "CompanyActivities");

            migrationBuilder.DropColumn(
                name: "EntityName",
                table: "CompanyActivities");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "CompanyActivities");

            migrationBuilder.DropColumn(
                name: "MetaJson",
                table: "CompanyActivities");

            migrationBuilder.DropColumn(
                name: "RefId",
                table: "CompanyActivities");
        }
    }
}
