using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using QuanLyNhaTro.Models;

namespace QuanLyNhaTro.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LoaiPhong> LoaiPhongs { get; set; }
        public DbSet<Phong> Phongs { get; set; }
        public DbSet<NguoiThue> NguoiThues { get; set; }
        public DbSet<HopDong> HopDongs { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình các mối quan hệ
            modelBuilder.Entity<Phong>()
                .HasOne(p => p.LoaiPhong)
                .WithMany()
                .HasForeignKey(p => p.LoaiPhongId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HopDong>()
                .HasOne(h => h.Phong)
                .WithMany()
                .HasForeignKey(h => h.PhongId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HopDong>()
                .HasOne(h => h.NguoiThue)
                .WithMany()
                .HasForeignKey(h => h.NguoiThueId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HoaDon>()
                .HasOne(h => h.HopDong)
                .WithMany()
                .HasForeignKey(h => h.HopDongId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}