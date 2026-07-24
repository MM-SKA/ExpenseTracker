using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Api.Migrations
{
    /// <inheritdoc />
    public partial class PhoneNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
            _ = migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
            _ = migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");
        }
    }
}
