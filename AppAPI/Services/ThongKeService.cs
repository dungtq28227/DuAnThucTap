using AppAPI.IServices;
using AppData.IRepositories;
using AppData.Models;
using AppData.Repositories;
using System;

namespace AppAPI.Services
{
    public class ThongKeService : IThongKeService
    {
        private readonly IAllRepository<HoaDon> repos;
         AssignmentDBContext context = new AssignmentDBContext();
        public ThongKeService()
        {
            repos = new AllRepository<HoaDon>(context, context.HoaDons);
        }
        public decimal DoanhThuNam(int year)
        {
            var nam=  context.HoaDons.Where(hd => hd.NgayTao.Year == year).ToList();
            decimal total = nam.Sum(hd => hd.TienShip);
            return total;
        }

        public decimal DoanhThuNgay(DateTime date)
        {
            var ngay= context.HoaDons.Where(hd => hd.NgayTao.Date == date.Date).ToList();
            decimal total = ngay.Sum(hd => hd.TienShip);
            return total;
        }

        public decimal DoanhThuThang(int month, int year)
        {
            var thang = context.HoaDons.Where(hd => hd.NgayTao.Month == month && hd.NgayTao.Year == year).ToList();
            decimal total = thang.Sum(hd => hd.TienShip);
            return total;
        }

    }
}
