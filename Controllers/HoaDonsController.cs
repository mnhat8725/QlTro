using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaTro.Data;
using QuanLyNhaTro.Models;

namespace QuanLyNhatro.Controllers
{
    [Authorize(Roles = "Admin,ChuTro")]
    public class HoaDonsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoaDonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HoaDons
        public async Task<IActionResult> Index(string searchString, int? thang, int? nam, TrangThaiHoaDon? trangThai)
        {
            ViewBag.SearchString = searchString;
            ViewBag.Thang = thang;
            ViewBag.Nam = nam;
            ViewBag.TrangThai = trangThai;

            var hoaDons = _context.HoaDons
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd!.Phong)
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd!.NguoiThue)
                .AsQueryable();

            // Lọc theo tên phòng hoặc người thuê
            if (!string.IsNullOrEmpty(searchString))
            {
                hoaDons = hoaDons.Where(h =>
                    h.HopDong!.Phong!.TenPhong.Contains(searchString) ||
                    h.HopDong!.NguoiThue!.HoTen.Contains(searchString));
            }

            // Lọc theo tháng
            if (thang.HasValue)
            {
                hoaDons = hoaDons.Where(h => h.ThangNam.Month == thang.Value);
            }

            // Lọc theo năm
            if (nam.HasValue)
            {
                hoaDons = hoaDons.Where(h => h.ThangNam.Year == nam.Value);
            }

            // Lọc theo trạng thái
            if (trangThai.HasValue)
            {
                hoaDons = hoaDons.Where(h => h.TrangThai == trangThai.Value);
            }

            var result = await hoaDons
                .OrderByDescending(h => h.ThangNam)
                .ThenBy(h => h.HopDong!.Phong!.TenPhong)
                .ToListAsync();

            // Thống kê
            ViewBag.TongSoHoaDon = result.Count;
            ViewBag.SoChuaThanhToan = result.Count(h => h.TrangThai == TrangThaiHoaDon.ChuaThanhToan);
            ViewBag.SoDaThanhToan = result.Count(h => h.TrangThai == TrangThaiHoaDon.DaThanhToan);
            ViewBag.TongTienChuaTT = result.Where(h => h.TrangThai == TrangThaiHoaDon.ChuaThanhToan).Sum(h => (decimal?)h.TongTien) ?? 0;
            ViewBag.TongTienDaTT = result.Where(h => h.TrangThai == TrangThaiHoaDon.DaThanhToan).Sum(h => (decimal?)h.TongTien) ?? 0;

            return View(result);
        }

        // GET: HoaDons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd!.Phong)
                        .ThenInclude(p => p!.LoaiPhong)
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd!.NguoiThue)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // GET: HoaDons/Create
        public IActionResult Create()
        {
            // Chỉ lấy hợp đồng đang hoạt động
            var hopDongs = _context.HopDongs
                .Include(h => h.Phong)
                .Include(h => h.NguoiThue)
                .Where(h => h.NgayKetThuc == null || h.NgayKetThuc > DateTime.Now)
                .Select(h => new
                {
                    h.Id,
                    Display = h.Phong!.TenPhong + " - " + h.NguoiThue!.HoTen
                })
                .ToList();

            ViewData["HopDongId"] = new SelectList(hopDongs, "Id", "Display");

            // Tháng mặc định là tháng hiện tại
            ViewBag.ThangNamMacDinh = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            return View();
        }

        // POST: HoaDons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HopDongId,ThangNam,TienPhong,ChiSoDienCu,ChiSoDienMoi,DonGiaDien,ChiSoNuocCu,ChiSoNuocMoi,DonGiaNuoc,TienDichVuKhac,GhiChu")] HoaDon hoaDon)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng hóa đơn
                var exists = await _context.HoaDons
                    .AnyAsync(h => h.HopDongId == hoaDon.HopDongId &&
                                   h.ThangNam.Year == hoaDon.ThangNam.Year &&
                                   h.ThangNam.Month == hoaDon.ThangNam.Month);

                if (exists)
                {
                    ModelState.AddModelError("", "Đã tồn tại hóa đơn cho hợp đồng này trong tháng đã chọn.");
                }
                else
                {
                    hoaDon.TrangThai = TrangThaiHoaDon.ChuaThanhToan;
                    hoaDon.NgayTao = DateTime.Now;

                    _context.Add(hoaDon);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Tạo hóa đơn thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Reload dropdown nếu có lỗi
            var hopDongs = _context.HopDongs
                .Include(h => h.Phong)
                .Include(h => h.NguoiThue)
                .Where(h => h.NgayKetThuc == null || h.NgayKetThuc > DateTime.Now)
                .Select(h => new
                {
                    h.Id,
                    Display = h.Phong!.TenPhong + " - " + h.NguoiThue!.HoTen
                })
                .ToList();

            ViewData["HopDongId"] = new SelectList(hopDongs, "Id", "Display", hoaDon.HopDongId);
            return View(hoaDon);
        }

        // GET: HoaDons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            var hopDongs = _context.HopDongs
                .Include(h => h.Phong)
                .Include(h => h.NguoiThue)
                .Select(h => new
                {
                    h.Id,
                    Display = h.Phong!.TenPhong + " - " + h.NguoiThue!.HoTen
                })
                .ToList();

            ViewData["HopDongId"] = new SelectList(hopDongs, "Id", "Display", hoaDon.HopDongId);
            return View(hoaDon);
        }

        // POST: HoaDons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HopDongId,ThangNam,TienPhong,ChiSoDienCu,ChiSoDienMoi,DonGiaDien,ChiSoNuocCu,ChiSoNuocMoi,DonGiaNuoc,TienDichVuKhac,GhiChu,TrangThai,NgayTao,NgayThanhToan")] HoaDon hoaDon)
        {
            if (id != hoaDon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDon);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật hóa đơn thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonExists(hoaDon.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var hopDongs = _context.HopDongs
                .Include(h => h.Phong)
                .Include(h => h.NguoiThue)
                .Select(h => new
                {
                    h.Id,
                    Display = h.Phong!.TenPhong + " - " + h.NguoiThue!.HoTen
                })
                .ToList();

            ViewData["HopDongId"] = new SelectList(hopDongs, "Id", "Display", hoaDon.HopDongId);
            return View(hoaDon);
        }

        // POST: HoaDons/MarkAsPaid/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            hoaDon.TrangThai = TrangThaiHoaDon.DaThanhToan;
            hoaDon.NgayThanhToan = DateTime.Now;

            _context.Update(hoaDon);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã đánh dấu hóa đơn là đã thanh toán!";
            return RedirectToAction(nameof(Index));
        }

        // GET: HoaDons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd!.Phong)
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd!.NguoiThue)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // POST: HoaDons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null)
            {
                _context.HoaDons.Remove(hoaDon);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa hóa đơn thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        // API: Lấy chỉ số điện/nước cũ
        [HttpGet]
        public async Task<IActionResult> GetLatestMeters(int hopDongId)
        {
            var latestHoaDon = await _context.HoaDons
                .Where(h => h.HopDongId == hopDongId)
                .OrderByDescending(h => h.ThangNam)
                .FirstOrDefaultAsync();

            if (latestHoaDon == null)
            {
                return Json(new { chiSoDienCu = 0, chiSoNuocCu = 0 });
            }

            return Json(new
            {
                chiSoDienCu = latestHoaDon.ChiSoDienMoi,
                chiSoNuocCu = latestHoaDon.ChiSoNuocMoi
            });
        }

        private bool HoaDonExists(int id)
        {
            return _context.HoaDons.Any(e => e.Id == id);
        }
    }
}
