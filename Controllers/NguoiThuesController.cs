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

namespace QuanLyNhatro.Controllers
{
    [Authorize]
    public class NguoiThuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NguoiThuesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.NguoiThues.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiThue = await _context.NguoiThues
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nguoiThue == null)
            {
                return NotFound();
            }

            return View(nguoiThue);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HoTen,SoCCCD,QueQuan,NoiLamViec,SoDienThoai")] NguoiThue nguoiThue, IFormFile? anhChanDung)
        {
            if (ModelState.IsValid)
            {
                if (anhChanDung != null && anhChanDung.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "nguoithue");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + anhChanDung.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await anhChanDung.CopyToAsync(fileStream);
                    }

                    nguoiThue.AnhChanDung = "/uploads/nguoithue/" + uniqueFileName;
                }

                _context.Add(nguoiThue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nguoiThue);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiThue = await _context.NguoiThues.FindAsync(id);
            if (nguoiThue == null)
            {
                return NotFound();
            }
            return View(nguoiThue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HoTen,SoCCCD,QueQuan,NoiLamViec,SoDienThoai,AnhChanDung")] NguoiThue nguoiThue, IFormFile? anhChanDung)
        {
            if (id != nguoiThue.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (anhChanDung != null && anhChanDung.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(nguoiThue.AnhChanDung))
                        {
                            string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, nguoiThue.AnhChanDung.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "nguoithue");
                        Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + anhChanDung.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await anhChanDung.CopyToAsync(fileStream);
                        }

                        nguoiThue.AnhChanDung = "/uploads/nguoithue/" + uniqueFileName;
                    }

                    _context.Update(nguoiThue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguoiThueExists(nguoiThue.Id))
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
            return View(nguoiThue);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiThue = await _context.NguoiThues
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nguoiThue == null)
            {
                return NotFound();
            }

            return View(nguoiThue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nguoiThue = await _context.NguoiThues.FindAsync(id);
            if (nguoiThue != null)
            {
                if (!string.IsNullOrEmpty(nguoiThue.AnhChanDung))
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, nguoiThue.AnhChanDung.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.NguoiThues.Remove(nguoiThue);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NguoiThueExists(int id)
        {
            return _context.NguoiThues.Any(e => e.Id == id);
        }
    }
}