using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Api.Migrations
{
    /// <inheritdoc />
    public partial class isSystemCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
            _ = migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories");

            _ = migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Categories",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            _ = migrationBuilder.AddColumn<bool>(
                name: "IsSystemCategory",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            _ = migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories");

            _ = migrationBuilder.DropColumn(
                name: "IsSystemCategory",
                table: "Categories");

            _ = migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
