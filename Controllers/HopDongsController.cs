using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaTro.Data;
using QuanLyNhaTro.Models;

namespace QuanLyNhaTro.Controllers
{
    [Authorize]
    public class HopDongsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HopDongsController(
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var hopDongs = await _context.HopDongs
                .Include(h => h.Phong)
                .Include(h => h.NguoiThue)
                .ToListAsync();
            return View(hopDongs);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var hopDong = await _context.HopDongs
                .Include(h => h.Phong)
                .Include(h => h.NguoiThue)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hopDong == null) return NotFound();

            return View(hopDong);
        }

        public IActionResult Create()
        {
            ViewData["PhongId"] = new SelectList(_context.Phongs, "Id", "TenPhong");
            ViewData["NguoiThueId"] = new SelectList(_context.NguoiThues, "Id", "HoTen");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PhongId,NguoiThueId,NgayBatDau,NgayKetThuc")] HopDong hopDong, IFormFile? anhHopDong)
        {
            if (ModelState.IsValid)
            {
                // Upload ảnh hợp đồng (nếu có)
                if (anhHopDong != null && anhHopDong.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "hopdongs");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + anhHopDong.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await anhHopDong.CopyToAsync(fileStream);
                    }

                    hopDong.AnhHopDongUrl = "/uploads/hopdongs/" + uniqueFileName;
                }

                // Lưu hợp đồng
                _context.Add(hopDong);
                await _context.SaveChangesAsync();

                // Cập nhật trạng thái phòng
                var phong = await _context.Phongs.FindAsync(hopDong.PhongId);
                if (phong != null)
                {
                    phong.TinhTrang = TinhTrangPhong.DaThue;
                    await _context.SaveChangesAsync();
                }

                // Thông báo thành công
                TempData["SuccessMessage"] = "Tạo hợp đồng thành công!";

                return RedirectToAction(nameof(Index));
            }

            ViewData["PhongId"] = new SelectList(_context.Phongs, "Id", "TenPhong", hopDong.PhongId);
            ViewData["NguoiThueId"] = new SelectList(_context.NguoiThues, "Id", "HoTen", hopDong.NguoiThueId);
            return View(hopDong);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong == null) return NotFound();

            ViewData["PhongId"] = new SelectList(_context.Phongs, "Id", "TenPhong", hopDong.PhongId);
            ViewData["NguoiThueId"] = new SelectList(_context.NguoiThues, "Id", "HoTen", hopDong.NguoiThueId);
            return View(hopDong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PhongId,NguoiThueId,NgayBatDau,NgayKetThuc,AnhHopDongUrl")] HopDong hopDong, IFormFile? anhHopDong)
        {
            if (id != hopDong.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (anhHopDong != null && anhHopDong.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(hopDong.AnhHopDongUrl))
                        {
                            string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, hopDong.AnhHopDongUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "hopdongs");
                        Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + anhHopDong.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await anhHopDong.CopyToAsync(fileStream);
                        }

                        hopDong.AnhHopDongUrl = "/uploads/hopdongs/" + uniqueFileName;
                    }

                    _context.Update(hopDong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HopDongExists(hopDong.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["PhongId"] = new SelectList(_context.Phongs, "Id", "TenPhong", hopDong.PhongId);
            ViewData["NguoiThueId"] = new SelectList(_context.NguoiThues, "Id", "HoTen", hopDong.NguoiThueId);
            return View(hopDong);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var hopDong = await _context.HopDongs
                .Include(h => h.Phong)
                .Include(h => h.NguoiThue)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hopDong == null) return NotFound();

            return View(hopDong);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong != null)
            {
                if (!string.IsNullOrEmpty(hopDong.AnhHopDongUrl))
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, hopDong.AnhHopDongUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                var phong = await _context.Phongs.FindAsync(hopDong.PhongId);
                if (phong != null)
                {
                    phong.TinhTrang = TinhTrangPhong.Trong;
                    await _context.SaveChangesAsync();
                }

                _context.HopDongs.Remove(hopDong);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HopDongExists(int id)
        {
            return _context.HopDongs.Any(e => e.Id == id);
        }
    }
}
