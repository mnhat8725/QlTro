using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaTro.Data;
using QuanLyNhaTro.Models;

namespace QuanLyNhaTro.Controllers
{
    [Authorize(Roles = "NguoiThue")]
    public class TenantController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TenantController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tenant/Index - Dashboard
        public async Task<IActionResult> Index()
        {
            var nguoiThue = await GetCurrentNguoiThueAsync();
            if (nguoiThue == null)
            {
                ViewBag.Message = "Không tìm thấy thông tin người thuê. Vui lòng liên hệ quản trị viên.";
                return View("NoAccess");
            }

            // Lấy hợp đồng hiện tại
            var hopDong = await _context.HopDongs
                .Include(h => h.Phong)
                    .ThenInclude(p => p!.LoaiPhong)
                .Where(h => h.NguoiThueId == nguoiThue.Id)
                .OrderByDescending(h => h.NgayBatDau)
                .FirstOrDefaultAsync();

            if (hopDong == null)
            {
                ViewBag.Message = "Bạn chưa có hợp đồng thuê nào.";
                return View("NoRoom");
            }

            // Thống kê
            var soHoaDonChuaTT = await _context.HoaDons
                .Where(h => h.HopDongId == hopDong.Id && h.TrangThai == TrangThaiHoaDon.ChuaThanhToan)
                .CountAsync();

            var hoaDonMoiNhat = await _context.HoaDons
                .Where(h => h.HopDongId == hopDong.Id)
                .OrderByDescending(h => h.ThangNam)
                .FirstOrDefaultAsync();

            // Tính số ngày còn lại của hợp đồng
            int? soNgayConLai = null;
            if (hopDong.NgayKetThuc.HasValue)
            {
                soNgayConLai = (hopDong.NgayKetThuc.Value - DateTime.Now).Days;
            }

            ViewBag.NguoiThue = nguoiThue;
            ViewBag.HopDong = hopDong;
            ViewBag.SoHoaDonChuaTT = soHoaDonChuaTT;
            ViewBag.HoaDonMoiNhat = hoaDonMoiNhat;
            ViewBag.SoNgayConLai = soNgayConLai;

            return View();
        }

        // GET: Tenant/MyRoom - Phòng của tôi
        public async Task<IActionResult> MyRoom()
        {
            var nguoiThue = await GetCurrentNguoiThueAsync();
            if (nguoiThue == null)
                return RedirectToAction(nameof(Index));

            var hopDong = await _context.HopDongs
                .Include(h => h.Phong)
                    .ThenInclude(p => p!.LoaiPhong)
                .Where(h => h.NguoiThueId == nguoiThue.Id)
                .OrderByDescending(h => h.NgayBatDau)
                .FirstOrDefaultAsync();

            if (hopDong == null)
            {
                ViewBag.Message = "Bạn chưa có phòng nào.";
                return View("NoRoom");
            }

            ViewBag.NguoiThue = nguoiThue;
            ViewBag.HopDong = hopDong;

            return View();
        }

        // GET: Tenant/MyBills - Hóa đơn của tôi
        public async Task<IActionResult> MyBills()
        {
            var nguoiThue = await GetCurrentNguoiThueAsync();
            if (nguoiThue == null)
                return RedirectToAction(nameof(Index));

            var hopDong = await _context.HopDongs
                .Include(h => h.Phong)
                .Where(h => h.NguoiThueId == nguoiThue.Id)
                .OrderByDescending(h => h.NgayBatDau)
                .FirstOrDefaultAsync();

            if (hopDong == null)
            {
                ViewBag.Message = "Bạn chưa có hợp đồng thuê.";
                return View("NoRoom");
            }

            var hoaDons = await _context.HoaDons
                .Where(h => h.HopDongId == hopDong.Id)
                .OrderByDescending(h => h.ThangNam)
                .ToListAsync();

            ViewBag.HopDong = hopDong;
            ViewBag.NguoiThue = nguoiThue;

            return View(hoaDons);
        }

        // GET: Tenant/BillDetails/5 - Chi tiết hóa đơn
        public async Task<IActionResult> BillDetails(int? id)
        {
            if (id == null)
                return NotFound();

            var nguoiThue = await GetCurrentNguoiThueAsync();
            if (nguoiThue == null)
                return RedirectToAction(nameof(Index));

            var hoaDon = await _context.HoaDons
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd!.Phong)
                        .ThenInclude(p => p!.LoaiPhong)
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd!.NguoiThue)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hoaDon == null)
                return NotFound();

            // Kiểm tra quyền: Chỉ xem được hóa đơn của mình
            if (hoaDon.HopDong!.NguoiThueId != nguoiThue.Id)
            {
                return Forbid();
            }

            return View(hoaDon);
        }

        // GET: Tenant/PaymentHistory - Lịch sử thanh toán
        public async Task<IActionResult> PaymentHistory()
        {
            var nguoiThue = await GetCurrentNguoiThueAsync();
            if (nguoiThue == null)
                return RedirectToAction(nameof(Index));

            var hopDong = await _context.HopDongs
                .Where(h => h.NguoiThueId == nguoiThue.Id)
                .OrderByDescending(h => h.NgayBatDau)
                .FirstOrDefaultAsync();

            if (hopDong == null)
            {
                return View("NoRoom");
            }

            var hoaDons = await _context.HoaDons
                .Where(h => h.HopDongId == hopDong.Id)
                .OrderByDescending(h => h.ThangNam)
                .ToListAsync();

            // Thống kê
            var tongDaThanhToan = hoaDons
                .Where(h => h.TrangThai == TrangThaiHoaDon.DaThanhToan)
                .Sum(h => h.TongTien);

            var tongChuaThanhToan = hoaDons
                .Where(h => h.TrangThai == TrangThaiHoaDon.ChuaThanhToan)
                .Sum(h => h.TongTien);

            var trungBinhThang = hoaDons.Any() ? hoaDons.Average(h => h.TongTien) : 0;

            ViewBag.TongDaThanhToan = tongDaThanhToan;
            ViewBag.TongChuaThanhToan = tongChuaThanhToan;
            ViewBag.TrungBinhThang = trungBinhThang;
            ViewBag.HopDong = hopDong;

            return View(hoaDons);
        }

        // GET: Tenant/Profile - Thông tin cá nhân
        public async Task<IActionResult> Profile()
        {
            var nguoiThue = await GetCurrentNguoiThueAsync();
            if (nguoiThue == null)
            {
                ViewBag.Message = "Không tìm thấy thông tin người thuê.";
                return View("NoAccess");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.Email = currentUser?.Email;

            return View(nguoiThue);
        }

        // Helper method: Lấy người thuê hiện tại
        private async Task<NguoiThue?> GetCurrentNguoiThueAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return null;

            // Tìm người thuê dựa trên số điện thoại hoặc email
            var nguoiThue = await _context.NguoiThues
                .FirstOrDefaultAsync(n =>
                    n.SoDienThoai == currentUser.PhoneNumber ||
                    n.SoDienThoai == currentUser.Email);

            return nguoiThue;
        }
    }
}