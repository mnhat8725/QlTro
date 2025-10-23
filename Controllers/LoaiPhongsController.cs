using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaTro.Data;
using QuanLyNhaTro.Models;

namespace QuanLyNhaTro.Controllers
{
    [Authorize]
    public class LoaiPhongsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoaiPhongsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Include Phongs để hiển thị số lượng phòng
            var loaiPhongs = await _context.LoaiPhongs
                .Include(l => l.Phongs)
                .ToListAsync();
            return View(loaiPhongs);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TenLoaiPhong,GiaPhong,DienTich,MoTaNoiThat")] LoaiPhong loaiPhong)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loaiPhong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaiPhong);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var loaiPhong = await _context.LoaiPhongs.FindAsync(id);
            if (loaiPhong == null) return NotFound();
            return View(loaiPhong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenLoaiPhong,GiaPhong,DienTich,MoTaNoiThat")] LoaiPhong loaiPhong)
        {
            if (id != loaiPhong.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiPhong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiPhongExists(loaiPhong.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(loaiPhong);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var loaiPhong = await _context.LoaiPhongs
                .Include(l => l.Phongs)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loaiPhong == null) return NotFound();
            return View(loaiPhong);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loaiPhong = await _context.LoaiPhongs
                .Include(l => l.Phongs)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loaiPhong != null)
            {
                // Kiểm tra xem có phòng nào đang sử dụng loại phòng này không
                if (loaiPhong.Phongs.Any())
                {
                    TempData["Error"] = $"Không thể xóa loại phòng này vì đang có {loaiPhong.Phongs.Count} phòng sử dụng. Hãy xóa các phòng trước.";
                    return RedirectToAction(nameof(Index));
                }

                _context.LoaiPhongs.Remove(loaiPhong);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoaiPhongExists(int id)
        {
            return _context.LoaiPhongs.Any(e => e.Id == id);
        }
    }
}