using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowPress.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFeaturedToSourceItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "SourceItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "SourceItems");
        }
    }
}
