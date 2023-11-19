using AppAPI.IServices;
using AppData.IRepositories;
using AppData.Models;
using AppData.Repositories;
using AppData.ViewModels;

namespace AppAPI.Services
{
    public class NhanVienService : INhanVienService
    {
        private readonly
        AssignmentDBContext _dbContext;

        public NhanVienService()
        {
            _dbContext = new AssignmentDBContext();
        }

        public bool Delete(Guid id)
        {
            var nv = _dbContext.NhanViens.FirstOrDefault(nv => nv.ID == id);
            if (nv != null)
            {
                _dbContext.NhanViens.Remove(nv);
                _dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public List<NhanVien> GetAll()
        {
            return _dbContext.NhanViens.ToList();
        }

        public bool Update(Guid id, string ten, string email, string password, string sdt, string diachi, int trangthai, Guid idvaitro)
        {
            var nv = _dbContext.NhanViens.FirstOrDefault(x => x.ID == id);
            if (nv != null)
            {
                nv.Ten = ten;
                nv.Email = email;
                nv.PassWord = password;
                nv.SDT = sdt;
                nv.DiaChi = diachi;
                nv.TrangThai = trangthai;
                nv.IDVaiTro = idvaitro;
                _dbContext.NhanViens.Update(nv);
                _dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<NhanVien> Add(string ten, string email, string password, string sdt, string diachi, int trangthai, Guid idvaitro)
        {
            NhanVien nv = new NhanVien()
            {
                ID = Guid.NewGuid(),
                Ten = ten,
                Email = email,
                PassWord = password,
                SDT = sdt,
                DiaChi = diachi,
                TrangThai = trangthai,
                IDVaiTro = idvaitro,
            };
            _dbContext.NhanViens.Add(nv);
            _dbContext.SaveChanges();
            return nv;

        }

        public NhanVien GetById(Guid id)
        {
            var nv = _dbContext.NhanViens.FirstOrDefault(nv => nv.ID == id);
            return nv;
        }
    }
}
