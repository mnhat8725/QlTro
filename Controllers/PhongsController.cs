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
    public class PhongsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhongsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Phongs.Include(p => p.LoaiPhong);
            return View(await applicationDbContext.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["LoaiPhongId"] = new SelectList(_context.LoaiPhongs, "Id", "TenLoaiPhong");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TenPhong,LoaiPhongId")] Phong phong)
        {
            phong.TinhTrang = TinhTrangPhong.Trong;

            if (ModelState.IsValid)
            {
                _context.Add(phong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LoaiPhongId"] = new SelectList(_context.LoaiPhongs, "Id", "TenLoaiPhong", phong.LoaiPhongId);
            return View(phong);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var phong = await _context.Phongs.FindAsync(id);
            if (phong == null) return NotFound();
            ViewData["LoaiPhongId"] = new SelectList(_context.LoaiPhongs, "Id", "TenLoaiPhong", phong.LoaiPhongId);
            return View(phong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenPhong,TinhTrang,LoaiPhongId")] Phong phong)
        {
            if (id != phong.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var hopDongHoatDong = await _context.HopDongs
                        .Where(h => h.PhongId == phong.Id)
                        .AnyAsync();

                    if (hopDongHoatDong && phong.TinhTrang == TinhTrangPhong.Trong)
                    {
                        ModelState.AddModelError("TinhTrang", "Không thể đổi về 'Trống' vì phòng đang có hợp đồng. Hãy xóa hợp đồng trước.");
                        ViewData["LoaiPhongId"] = new SelectList(_context.LoaiPhongs, "Id", "TenLoaiPhong", phong.LoaiPhongId);
                        return View(phong);
                    }

                    if (hopDongHoatDong && phong.TinhTrang == TinhTrangPhong.DangSuaChua)
                    {
                        ModelState.AddModelError("TinhTrang", "Phòng đang có người thuê. Hãy xóa hợp đồng trước khi sửa chữa.");
                        ViewData["LoaiPhongId"] = new SelectList(_context.LoaiPhongs, "Id", "TenLoaiPhong", phong.LoaiPhongId);
                        return View(phong);
                    }

                    _context.Update(phong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhongExists(phong.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LoaiPhongId"] = new SelectList(_context.LoaiPhongs, "Id", "TenLoaiPhong", phong.LoaiPhongId);
            return View(phong);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var phong = await _context.Phongs
                .Include(p => p.LoaiPhong)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (phong == null) return NotFound();
            return View(phong);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong != null)
            {
                var coHopDong = await _context.HopDongs.AnyAsync(h => h.PhongId == id);
                if (coHopDong)
                {
                    TempData["Error"] = "Không thể xóa phòng vì đang có hợp đồng. Hãy xóa hợp đồng trước.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Phongs.Remove(phong);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhongExists(int id)
        {
            return _context.Phongs.Any(e => e.Id == id);
        }
    }
}