using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaTro.Models
{
    public class NguoiThue
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Họ Tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số CCCD là bắt buộc")]
        [StringLength(12)]
        [Display(Name = "Số CCCD/CMND")]
        public string SoCCCD { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Quê Quán")]
        public string? QueQuan { get; set; }

        [StringLength(200)]
        [Display(Name = "Nơi Làm Việc/Học Tập")]
        public string? NoiLamViec { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(15)]
        [Display(Name = "Số Điện Thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Display(Name = "Ảnh Chân Dung")]
        public string? AnhChanDung { get; set; }
        [StringLength(450)]
        public string? UserId { get; set; }

        public ICollection<HopDong> HopDongs { get; set; } = new List<HopDong>();
    }
}