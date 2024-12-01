using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace functions.Migrations
{
    /// <inheritdoc />
    public partial class longGroupColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Color",
                table: "User_Group",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Color",
                table: "User_Group",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
