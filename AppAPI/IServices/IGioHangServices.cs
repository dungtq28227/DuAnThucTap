using AppData.Models;

namespace AppAPI.IServices
{
    public interface IGioHangServices
    {
        public bool Add(Guid IdKhachHang, DateTime ngaytao);
        public bool Update(Guid Id, DateTime ngaytao);
        public bool Delete(Guid Id);
        public GioHang GetById(Guid Id);
        public List<GioHang> GetAll();
    }
}
