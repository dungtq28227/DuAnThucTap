using AppAPI.IServices;
using AppData.IRepositories;
using AppData.Models;
using AppData.Repositories;

namespace AppAPI.Services
{
    public class GioHangServices : IGioHangServices
    {
        private readonly IAllRepository<GioHang> repos;
        AssignmentDBContext context = new AssignmentDBContext();
        public GioHangServices()
        {
            repos = new AllRepository<GioHang>(context, context.GioHangs);
        }
        public bool Add(Guid IdKhachHang, DateTime ngaytao)
        {
            GioHang gioHang = new GioHang();
            gioHang.IDKhachHang = IdKhachHang;
            gioHang.NgayTao = ngaytao;
            return repos.Add(gioHang);
        }

        public bool Delete(Guid Id)
        {
            var gioHang = repos.GetAll().FirstOrDefault(x => x.IDKhachHang == Id);
            if (gioHang != null)
            {
                return repos.Delete(gioHang);
            }
            else
            {
                return false;
            }
        }

        public List<GioHang> GetAll()
        {
            return repos.GetAll().ToList();
        }

        public GioHang GetById(Guid Id)
        {
            return repos.GetAll().FirstOrDefault(x => x.IDKhachHang == Id);
        }

        public bool Update(Guid Id, DateTime ngaytao)
        {
            var gioHang = repos.GetAll().FirstOrDefault(x => x.IDKhachHang == Id);
            if (gioHang != null)
            {
                gioHang.NgayTao = ngaytao;
                return repos.Update(gioHang);
            }
            else
            {
                return false;
            }
        }
    }
}
