using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhatro.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToNguoiThue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "NguoiThues",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "NguoiThues");
        }
    }
}
