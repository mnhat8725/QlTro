using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhatro.Migrations
{
    /// <inheritdoc />
    public partial class AddAnhChanDungToNguoiThue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HopDongs_NguoiThues_NguoiThueId",
                table: "HopDongs");

            migrationBuilder.DropForeignKey(
                name: "FK_HopDongs_Phongs_PhongId",
                table: "HopDongs");

            migrationBuilder.DropForeignKey(
                name: "FK_Phongs_LoaiPhongs_LoaiPhongId",
                table: "Phongs");

            migrationBuilder.AddColumn<int>(
                name: "LoaiPhongId1",
                table: "Phongs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnhChanDung",
                table: "NguoiThues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiThueId1",
                table: "HopDongs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhongId1",
                table: "HopDongs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HoaDons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HopDongId = table.Column<int>(type: "int", nullable: false),
                    ThangNam = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TienPhong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ChiSoDienCu = table.Column<int>(type: "int", nullable: false),
                    ChiSoDienMoi = table.Column<int>(type: "int", nullable: false),
                    DonGiaDien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ChiSoNuocCu = table.Column<int>(type: "int", nullable: false),
                    ChiSoNuocMoi = table.Column<int>(type: "int", nullable: false),
                    DonGiaNuoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TienDichVuKhac = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoaDons_HopDongs_HopDongId",
                        column: x => x.HopDongId,
                        principalTable: "HopDongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Phongs_LoaiPhongId1",
                table: "Phongs",
                column: "LoaiPhongId1");

            migrationBuilder.CreateIndex(
                name: "IX_HopDongs_NguoiThueId1",
                table: "HopDongs",
                column: "NguoiThueId1");

            migrationBuilder.CreateIndex(
                name: "IX_HopDongs_PhongId1",
                table: "HopDongs",
                column: "PhongId1");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_HopDongId",
                table: "HoaDons",
                column: "HopDongId");

            migrationBuilder.AddForeignKey(
                name: "FK_HopDongs_NguoiThues_NguoiThueId",
                table: "HopDongs",
                column: "NguoiThueId",
                principalTable: "NguoiThues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HopDongs_NguoiThues_NguoiThueId1",
                table: "HopDongs",
                column: "NguoiThueId1",
                principalTable: "NguoiThues",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HopDongs_Phongs_PhongId",
                table: "HopDongs",
                column: "PhongId",
                principalTable: "Phongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HopDongs_Phongs_PhongId1",
                table: "HopDongs",
                column: "PhongId1",
                principalTable: "Phongs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Phongs_LoaiPhongs_LoaiPhongId",
                table: "Phongs",
                column: "LoaiPhongId",
                principalTable: "LoaiPhongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Phongs_LoaiPhongs_LoaiPhongId1",
                table: "Phongs",
                column: "LoaiPhongId1",
                principalTable: "LoaiPhongs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HopDongs_NguoiThues_NguoiThueId",
                table: "HopDongs");

            migrationBuilder.DropForeignKey(
                name: "FK_HopDongs_NguoiThues_NguoiThueId1",
                table: "HopDongs");

            migrationBuilder.DropForeignKey(
                name: "FK_HopDongs_Phongs_PhongId",
                table: "HopDongs");

            migrationBuilder.DropForeignKey(
                name: "FK_HopDongs_Phongs_PhongId1",
                table: "HopDongs");

            migrationBuilder.DropForeignKey(
                name: "FK_Phongs_LoaiPhongs_LoaiPhongId",
                table: "Phongs");

            migrationBuilder.DropForeignKey(
                name: "FK_Phongs_LoaiPhongs_LoaiPhongId1",
                table: "Phongs");

            migrationBuilder.DropTable(
                name: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_Phongs_LoaiPhongId1",
                table: "Phongs");

            migrationBuilder.DropIndex(
                name: "IX_HopDongs_NguoiThueId1",
                table: "HopDongs");

            migrationBuilder.DropIndex(
                name: "IX_HopDongs_PhongId1",
                table: "HopDongs");

            migrationBuilder.DropColumn(
                name: "LoaiPhongId1",
                table: "Phongs");

            migrationBuilder.DropColumn(
                name: "AnhChanDung",
                table: "NguoiThues");

            migrationBuilder.DropColumn(
                name: "NguoiThueId1",
                table: "HopDongs");

            migrationBuilder.DropColumn(
                name: "PhongId1",
                table: "HopDongs");

            migrationBuilder.AddForeignKey(
                name: "FK_HopDongs_NguoiThues_NguoiThueId",
                table: "HopDongs",
                column: "NguoiThueId",
                principalTable: "NguoiThues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HopDongs_Phongs_PhongId",
                table: "HopDongs",
                column: "PhongId",
                principalTable: "Phongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Phongs_LoaiPhongs_LoaiPhongId",
                table: "Phongs",
                column: "LoaiPhongId",
                principalTable: "LoaiPhongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
