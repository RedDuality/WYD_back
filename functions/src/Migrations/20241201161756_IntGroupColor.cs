using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace functions.Migrations
{
    /// <inheritdoc />
    public partial class IntGroupColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Color",
                table: "User_Group",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "User_Group",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
