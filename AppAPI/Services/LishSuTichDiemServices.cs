using AppAPI.IServices;
using AppData.IRepositories;
using AppData.Models;
using AppData.Repositories;
using AppData.ViewModels;
using AppData.ViewModels.SanPham;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Services
{
    public class LishSuTichDiemServices : ILishSuTichDiemServices
    {
        private readonly IAllRepository<LichSuTichDiem> _allRepository;
        AssignmentDBContext context= new AssignmentDBContext();
        public LishSuTichDiemServices()
        {
            _allRepository= new AllRepository<LichSuTichDiem>(context,context.LichSuTichDiems);
        }
        public bool Add(int diem, int trangthai, Guid IdKhachHang, Guid IdQuyDoiDiem, Guid IdHoaDon)
        {
            var lichsu = new LichSuTichDiem();
            lichsu.ID=Guid.NewGuid();
            lichsu.Diem=diem;
            lichsu.TrangThai=trangthai;
            lichsu.IDKhachHang=IdKhachHang;
            lichsu.IDQuyDoiDiem = IdQuyDoiDiem;
            lichsu.IDHoaDon = IdHoaDon;
            return _allRepository.Add(lichsu);
        }

        public bool Delete(Guid Id)
        {
            var lichsu = _allRepository.GetAll().First(x => x.ID == Id);
            if (lichsu != null)
            {
               
                return _allRepository.Delete(lichsu);
            }
            else
            {
                return false;
            }
        }

        public List<LichSuTichDiem> GetAll()
        {
            return _allRepository.GetAll(); 
        }
       

        public LichSuTichDiem GetById(Guid Id)
        {
            return _allRepository.GetAll().First(x => x.ID==Id);
        }

        public bool Update(Guid Id, int diem, int trangthai, Guid IdKhachHang, Guid IdQuyDoiDiem, Guid IdHoaDon)
        {
            var lichsu= _allRepository.GetAll().First(x => x.ID == Id);
            if (lichsu != null)
            {
                lichsu.Diem = diem;
                lichsu.TrangThai = trangthai;
                lichsu.IDKhachHang = IdKhachHang;
                lichsu.IDQuyDoiDiem = IdQuyDoiDiem;
                lichsu.IDHoaDon = IdHoaDon;
                return _allRepository.Update(lichsu);
            }
            else
            {
                return false;
            }
        }
        //dũng
        public async Task<List<DonMuaViewModel>> getAllDonMua(Guid idKhachHang)
        {
            try
            {
                var lstDonMua = await(from a in context.LichSuTichDiems
                                       join b in context.HoaDons on a.IDHoaDon equals b.ID
                                       join d in context.KhachHangs on a.IDKhachHang equals d.IDKhachHang
                                       join e in context.QuyDoiDiems on a.IDQuyDoiDiem equals e.ID
                                       join c in context.Vouchers on b.IDVoucher equals c.ID
                                      select new DonMuaViewModel()
                                       {
                                           IDLichSu = a.ID,
                                           Diem = a.Diem,
                                           TrangThaiLSTD = a.TrangThai,
                                           IDHoaDon = b.ID,
                                           NgayTao = b.NgayTao,
                                           NgayThanhToan = b.NgayThanhToan,
                                           TenNguoiNhan = b.TenNguoiNhan,
                                           SDT = b.SDT,
                                           Email = b.Email,
                                           DiaChi = b.DiaChi,   
                                           TienShip = b.TienShip,
                                           TrangThaiGiaoHang = b.TrangThaiGiaoHang,
                                           IDVoucher = c.ID,
                                           HinhThucGiamGia = c.HinhThucGiamGia,
                                           GiaTri= c.GiaTri,
                                           IdNguoiDung = d.IDKhachHang,
                                           TiLeTichDiem = e.TiLeTichDiem,
                                           TiLeTieuDiem = e.TiLeTieuDiem,
                                           TongTien =0
                                       }).ToListAsync();
                foreach (var item in lstDonMua)
                {
                    List<ChiTietHoaDon> cthd = context.ChiTietHoaDons.Where(p => p.IDHoaDon == item.IDHoaDon).ToList();
                    foreach (var x in cthd)
                    {
                        item.TongTien += x.DonGia * x.SoLuong;
                    }
                    if (item.HinhThucGiamGia == 0)
                    {
                        item.TongTien = (item.TongTien * (100 - item.GiaTri / 100))+item.TienShip;
                    }
                    else if (item.HinhThucGiamGia == 1)
                    {
                        item.TongTien = (item.TongTien - item.GiaTri) + item.TienShip;
                    }
                    if (item.TrangThaiLSTD == 0)
                    {
                        item.TongTien = item.TongTien - (item.TiLeTieuDiem*item.Diem);
                    }
                }
                lstDonMua = lstDonMua.Where(P => P.IdNguoiDung == idKhachHang).ToList();
                return lstDonMua;
            }
            catch
            {
                return new List<DonMuaViewModel>();
            }

        }

        public async Task<List<DonMuaChiTietViewModel>> getAllDonMuaChiTiet(Guid idHoaDon)
        {
            try
            {
                var lstDonMuaCT = await(from a in context.HoaDons
                                      join b in context.LichSuTichDiems on a.ID equals b.IDHoaDon
                                      join c in context.ChiTietHoaDons on a.ID equals c.IDHoaDon
                                      join d in context.DanhGias on c.ID equals d.ID
                                      join e in context.ChiTietSanPhams on c.IDCTSP equals e.ID
                                      join f in context.KichCos on e.IDKichCo equals f.ID
                                      join g in context.MauSacs on e.IDMauSac equals g.ID
                                      join i in context.SanPhams on e.IDSanPham equals i.ID
                                      join h in context.Anhs on i.ID equals h.IDSanPham
                                      join k in context.Vouchers on a.IDVoucher equals k.ID
                                      join l in context.QuyDoiDiems on b.IDQuyDoiDiem equals l.ID
                                      select new DonMuaChiTietViewModel()
                                      {
                                         ID = a.ID,
                                         NgayTao = a.NgayTao,
                                         NgayThanhToan = a.NgayThanhToan,
                                         TenNguoiNhan = a.TenNguoiNhan,
                                         SDT = a.SDT,
                                         Email = a.Email,
                                         DiaChi = a.DiaChi,
                                         TienShip = a.TienShip,
                                         PhuongThucThanhToan = a.PhuongThucThanhToan,
                                         TrangThaiGiaoHang = a.TrangThaiGiaoHang,
                                         Diem = b.Diem,
                                         TrangThaiLichSuTichDiem = b.TrangThai,
                                         IDCTHD = c.ID,
                                         DonGia = c.DonGia,
                                         SoLuong = c.SoLuong,
                                         TenKichCo = f.Ten,
                                         TenMau = g.Ten,
                                         TenSanPham = i.Ten,
                                         DuongDan = h.DuongDan,
                                         HinhThucGiamGia = k.HinhThucGiamGia,
                                         GiaTri = k.GiaTri,
                                         TiLeTieuDiem = l.TiLeTieuDiem,
                                         TrangThaiDanhGia = d.TrangThai
                                      }).ToListAsync();
                
                
               
                
                return lstDonMuaCT.Where(p=>p.ID == idHoaDon).ToList();
            }
            catch
            {
                return new List<DonMuaChiTietViewModel>();
            }
        }

        public async Task<ChiTietHoaDonDanhGiaViewModel> getCTHDDanhGia(Guid idcthd)
        {
            try
            {
                var getCTHDDanhGia = (from a in context.ChiTietHoaDons.Where(p=>p.ID == idcthd)
                                        join b in context.ChiTietSanPhams on a.IDCTSP equals b.ID
                                        join c in context.KichCos on b.IDKichCo equals c.ID
                                        join d in context.MauSacs on b.IDMauSac equals d.ID
                                        join e in context.SanPhams on b.IDSanPham equals e.ID
                                        join f in context.Anhs on e.ID equals f.IDSanPham
                                        join g in context.HoaDons on a.IDHoaDon equals g.ID
                                       
                                        select new ChiTietHoaDonDanhGiaViewModel()
                                        {
                                            ID = a.ID,
                                            TenSanPham = e.Ten,
                                            TenMau = d.Ten,
                                            TenKichThuoc = c.Ten,
                                            DuongDan = f.DuongDan,
                                            IDHoaDon = g.ID
                                            
                                        }).FirstOrDefault();




                return getCTHDDanhGia;
            }
            catch
            {
                return new ChiTietHoaDonDanhGiaViewModel();
            }
        }
    }
}
