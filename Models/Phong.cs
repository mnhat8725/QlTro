using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaTro.Models
{
    public enum TinhTrangPhong
    {
        [Display(Name = "Còn Trống")]
        Trong,
        [Display(Name = "Đã Thuê")]
        DaThue,
        [Display(Name = "Đang Sửa Chữa")]
        DangSuaChua
    }

    public class Phong
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên phòng là bắt buộc")]
        [StringLength(50)]
        [Display(Name = "Tên Phòng")]
        public string TenPhong { get; set; } = string.Empty;

        [Display(Name = "Tình Trạng")]
        public TinhTrangPhong TinhTrang { get; set; } = TinhTrangPhong.Trong;

        [Display(Name = "Loại Phòng")]
        public int LoaiPhongId { get; set; }

        public LoaiPhong? LoaiPhong { get; set; }
        public ICollection<HopDong> HopDongs { get; set; } = new List<HopDong>();
    }
}