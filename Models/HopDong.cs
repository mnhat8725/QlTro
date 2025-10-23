using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaTro.Models
{
    public class HopDong
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phòng")]
        [Display(Name = "Phòng")]
        public int PhongId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn người thuê")]
        [Display(Name = "Người Thuê")]
        public int NguoiThueId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [Display(Name = "Ngày Bắt Đầu Thuê")]
        public DateTime NgayBatDau { get; set; }

        [Display(Name = "Ngày Kết Thúc")]
        public DateTime? NgayKetThuc { get; set; }

        [Display(Name = "Ảnh Hợp Đồng")]
        public string? AnhHopDongUrl { get; set; }
        public Phong? Phong { get; set; }
        public NguoiThue? NguoiThue { get; set; }
    }
}