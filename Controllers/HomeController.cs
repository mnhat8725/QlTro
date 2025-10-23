using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhatro.Models;
using QuanLyNhaTro.Data;
using QuanLyNhaTro.Models;
using System.Diagnostics;

namespace QuanLyNhaTro.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TongSoPhong = await _context.Phongs.CountAsync(),
                PhongDaThue = await _context.Phongs.CountAsync(p => p.TinhTrang == TinhTrangPhong.DaThue),
                TongNguoiThue = await _context.NguoiThues.CountAsync(),
                DoanhThuDuKien = await _context.Phongs
                                            .Where(p => p.TinhTrang == TinhTrangPhong.DaThue && p.LoaiPhong != null)
                                            .SumAsync(p => p.LoaiPhong!.GiaPhong)
            };
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public class DashboardViewModel
        {
            public int TongSoPhong { get; set; }
            public int PhongDaThue { get; set; }
            public int TongNguoiThue { get; set; }
            public decimal DoanhThuDuKien { get; set; }
        }
    }
}