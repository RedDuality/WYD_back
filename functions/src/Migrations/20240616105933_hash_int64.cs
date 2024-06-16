using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace functions.Migrations
{
    /// <inheritdoc />
    public partial class hash_int64 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_Hash",
                table: "Events");

            migrationBuilder.AlterColumn<long>(
                name: "Hash",
                table: "Events",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Hash",
                table: "Events",
                column: "Hash",
                unique: true,
                filter: "[Hash] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_Hash",
                table: "Events");

            migrationBuilder.AlterColumn<int>(
                name: "Hash",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_Hash",
                table: "Events",
                column: "Hash",
                unique: true);
        }
    }
}
