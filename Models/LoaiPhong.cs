using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaTro.Models
{
    public class LoaiPhong
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên loại phòng là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Tên Loại Phòng")]
        public string TenLoaiPhong { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Giá phòng là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phòng phải là số dương")]
        [Display(Name = "Giá Phòng")]
        public decimal GiaPhong { get; set; }

        [Required(ErrorMessage = "Diện tích là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Diện tích phải là số dương")]
        [Display(Name = "Diện Tích (m²)")]
        public float DienTich { get; set; }

        [Display(Name = "Mô Tả Nội Thất")]
        public string? MoTaNoiThat { get; set; }

        public ICollection<Phong> Phongs { get; set; } = new List<Phong>();
    }
}