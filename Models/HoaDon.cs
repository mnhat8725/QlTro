
using System.ComponentModel.DataAnnotations;
namespace QuanLyNhaTro.Models
{
    public enum TrangThaiHoaDon
    {
        [Display(Name = "Chưa Thanh Toán")]
        ChuaThanhToan = 0,

        [Display(Name = "Đã Thanh Toán")]
        DaThanhToan = 1,

        [Display(Name = "Quá Hạn")]
        QuaHan = 2
    }
    public class HoaDon
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hợp đồng")]
        [Display(Name = "Hợp Đồng")]
        public int HopDongId { get; set; }

        [Required(ErrorMessage = "Tháng/năm là bắt buộc")]
        [Display(Name = "Tháng/Năm")]
        [DataType(DataType.Date)]
        public DateTime ThangNam { get; set; }

        [Required(ErrorMessage = "Tiền phòng là bắt buộc")]
        [Display(Name = "Tiền Phòng")]
        [DataType(DataType.Currency)]
        public decimal TienPhong { get; set; }

        // ===== ĐIỆN =====
        [Required(ErrorMessage = "Chỉ số điện cũ là bắt buộc")]
        [Display(Name = "Chỉ Số Điện Cũ")]
        [Range(0, int.MaxValue, ErrorMessage = "Chỉ số điện phải >= 0")]
        public int ChiSoDienCu { get; set; }

        [Required(ErrorMessage = "Chỉ số điện mới là bắt buộc")]
        [Display(Name = "Chỉ Số Điện Mới")]
        [Range(0, int.MaxValue, ErrorMessage = "Chỉ số điện phải >= 0")]
        public int ChiSoDienMoi { get; set; }

        [Required(ErrorMessage = "Đơn giá điện là bắt buộc")]
        [Display(Name = "Đơn Giá Điện (đ/kWh)")]
        [DataType(DataType.Currency)]
        public decimal DonGiaDien { get; set; } = 3500;

        // ===== NƯỚC =====
        [Required(ErrorMessage = "Chỉ số nước cũ là bắt buộc")]
        [Display(Name = "Chỉ Số Nước Cũ")]
        [Range(0, int.MaxValue, ErrorMessage = "Chỉ số nước phải >= 0")]
        public int ChiSoNuocCu { get; set; }

        [Required(ErrorMessage = "Chỉ số nước mới là bắt buộc")]
        [Display(Name = "Chỉ Số Nước Mới")]
        [Range(0, int.MaxValue, ErrorMessage = "Chỉ số nước phải >= 0")]
        public int ChiSoNuocMoi { get; set; }

        [Required(ErrorMessage = "Đơn giá nước là bắt buộc")]
        [Display(Name = "Đơn Giá Nước (đ/m³)")]
        [DataType(DataType.Currency)]
        public decimal DonGiaNuoc { get; set; } = 20000;

        // ===== DỊCH VỤ KHÁC =====
        [Display(Name = "Tiền Dịch Vụ Khác")]
        [DataType(DataType.Currency)]
        public decimal? TienDichVuKhac { get; set; } = 100000;

        [Display(Name = "Ghi Chú")]
        [DataType(DataType.MultilineText)]
        public string? GhiChu { get; set; }

        // ===== TRẠNG THÁI =====
        [Display(Name = "Trạng Thái")]
        public TrangThaiHoaDon TrangThai { get; set; } = TrangThaiHoaDon.ChuaThanhToan;

        [Display(Name = "Ngày Tạo")]
        [DataType(DataType.DateTime)]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Ngày Thanh Toán")]
        [DataType(DataType.DateTime)]
        public DateTime? NgayThanhToan { get; set; }

        // ===== CALCULATED PROPERTIES =====
        [Display(Name = "Số Điện Tiêu Thụ (kWh)")]
        public int SoDienTieuThu => ChiSoDienMoi - ChiSoDienCu;

        [Display(Name = "Tiền Điện")]
        public decimal TienDien => SoDienTieuThu * DonGiaDien;

        [Display(Name = "Số Nước Tiêu Thụ (m³)")]
        public int SoNuocTieuThu => ChiSoNuocMoi - ChiSoNuocCu;

        [Display(Name = "Tiền Nước")]
        public decimal TienNuoc => SoNuocTieuThu * DonGiaNuoc;

        [Display(Name = "Tổng Tiền")]
        public decimal TongTien => TienPhong + TienDien + TienNuoc + (TienDichVuKhac ?? 0);

        // ===== NAVIGATION PROPERTIES =====
        public HopDong? HopDong { get; set; }
    }
}
