using AppAPI.IServices;
using AppData.Models;
using AppData.ViewModels;
using AppData.ViewModels.BanOffline;
using AppData.ViewModels.SanPham;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AppAPI.Services
{
    public class SanPhamService : ISanPhamService
    {
        private readonly AssignmentDBContext _context;
        public SanPhamService()
        {
            this._context = new AssignmentDBContext();
        }

        #region SanPham
        public Task<bool> UpdateSanPham(SanPhamRequest request)
        {
            throw new NotImplementedException();
        }
        public Task<List<SanPhamViewModel>> TimKiemSanPham(SanPhamTimKiemNangCao sp)
        {
            throw new NotImplementedException();
        }
        public Task<List<SanPhamViewModel>> GetSanPhamByIdDanhMuc(Guid idloaisp)
        {
            throw new NotImplementedException();
        }
        public Task<SanPhamDetail> GetSanPhamById(Guid id)
        {
            throw new NotImplementedException();
        }
        public async Task<List<SanPhamViewModel>> GetAllSanPham()
        {
            try
            {
                var lstSanPham = await (from a in _context.SanPhams.Where(x => x.TrangThai == 1)
                                        join b in _context.ChiTietSanPhams.Where(x => x.TrangThai == 1) on a.ID equals b.IDSanPham
                                        //join c in _context.Anhs.Where(x => x.TrangThai == 1) on a.ID equals c.IDSanPham
                                        join e in _context.LoaiSPs.Where(x => x.LoaiSPCha != null) on a.IDLoaiSP equals e.ID
                                        select new SanPhamViewModel()
                                        {
                                            ID = a.ID,
                                            Ten = a.Ten,
                                            TrangThai = a.TrangThai,
                                            TrangThaiCTSP = b.TrangThai,
                                            LoaiSP = e.Ten,
                                            IdChiTietSanPham = b.ID,
                                            Image = _context.Anhs.First(x => x.IDMauSac == b.IDMauSac && x.IDSanPham == a.ID).DuongDan,
                                            IDMauSac = b.IDMauSac,
                                            IDKichCo = b.IDKichCo,
                                            IDChatLieu = a.IDChatLieu,
                                            GiaGoc = b.GiaBan,
                                            NgayTao = b.NgayTao,
                                            GiaBan = b.IDKhuyenMai == null ? b.GiaBan : b.GiaBan * (100 - (_context.KhuyenMais.First(x => x.ID == b.IDKhuyenMai).GiaTri)) / 100
                                        }).ToListAsync();
                return lstSanPham;
            }
            catch
            {
                return new List<SanPhamViewModel>();
            }
        }
        public bool CheckTrungTenSP(SanPhamRequest lsp)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> DeleteSanPham(Guid id)
        {
            try
            {
                var sanPham = await _context.SanPhams.FindAsync(id);
                sanPham.TrangThai = 0;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<ChiTietSanPhamUpdateRequest> AddSanPham(SanPhamRequest request)
        {
            try
            {
                var first = true;
                List<ChiTietSanPhamRequest> lst = new List<ChiTietSanPhamRequest>();
                LoaiSP? loaiSPCon = _context.LoaiSPs.Where(x => x.IDLoaiSPCha != null).FirstOrDefault(x => x.Ten == request.TenLoaiSPCon);
                ChatLieu? chatLieu = _context.ChatLieus.FirstOrDefault(x => x.Ten == request.TenChatLieu);
                if (loaiSPCon == null)
                {
                    LoaiSP? loaiSPCha = _context.LoaiSPs.Where(x => x.IDLoaiSPCha == null).FirstOrDefault(x => x.Ten == request.TenLoaiSPCha);
                    if (loaiSPCha == null)
                    {
                        loaiSPCha = new LoaiSP() { ID = Guid.NewGuid(), Ten = request.TenLoaiSPCha, TrangThai = 1 };
                        _context.LoaiSPs.AddAsync(loaiSPCha);
                    }
                    loaiSPCon = new LoaiSP() { ID = Guid.NewGuid(), Ten = request.TenLoaiSPCon, IDLoaiSPCha = loaiSPCha.ID, TrangThai = 1 };
                    await _context.LoaiSPs.AddAsync(loaiSPCon);
                }
                if (chatLieu == null)
                {
                    chatLieu = new ChatLieu() { ID = Guid.NewGuid(), Ten = request.TenChatLieu, TrangThai = 1 };
                    await _context.AddAsync(chatLieu);
                }
                SanPham sanPham = new SanPham() { ID = Guid.NewGuid(), Ten = request.Ten, MoTa = request.MoTa, TrangThai = 1, TongDanhGia = 0, TongSoSao = 0, IDLoaiSP = loaiSPCon.ID, IDChatLieu = chatLieu.ID };
                await _context.SanPhams.AddAsync(sanPham);
                await _context.SaveChangesAsync();
                foreach (var x in request.MauSacs)
                {
                    foreach (var y in request.KichCos)
                    {
                        lst.Add(CreateChiTietSanPhamFromSanPham(x, y).Result);
                    }
                }
                return new ChiTietSanPhamUpdateRequest() { IDSanPham = sanPham.ID, ChiTietSanPhams = lst };
            }
            catch { return new ChiTietSanPhamUpdateRequest(); }
        }
        public async Task<bool> AddAnhToSanPham(List<AnhRequest> request)
        {
            try
            {
                foreach (var x in request)
                {
                    Anh anh = new Anh() { ID = Guid.NewGuid(), DuongDan = x.DuongDan, IDSanPham = x.IDSanPham, IDMauSac = _context.MauSacs.First(y => y.Ma == x.MaMau).ID, TrangThai = 1 };
                    _context.Anhs.Add(anh);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        //public async Task<bool> AddAnhToSanPhamFromSanPham(Guid idSanPham,List<AnhRequest> anhs)
        //{
        //    try
        //    {
        //        foreach (var x in anhs)
        //        {
        //            _context.Anhs.Add(new Anh() { ID = Guid.NewGuid(), DuongDan = x.DuongDan, IDMauSac = x.IDMauSac, IDSanPham = idSanPham, TrangThai = 1 });
        //        }
        //        await _context.SaveChangesAsync();
        //        return true;
        //    }
        //    catch { return false; }           
        //}
        #endregion

        #region ChiTietSanPham
        public async Task<ChiTietSanPhamViewModel> GetChiTietSanPhamByID(Guid id)
        {
            var temp = _context.ChiTietSanPhams.First(x => x.ID == id);
            ChiTietSanPhamViewModel chiTietSanPham = new ChiTietSanPhamViewModel() { ID = temp.ID, Ten = _context.SanPhams.First(x => x.ID == temp.IDSanPham).Ten, SoLuong = temp.SoLuong, GiaBan = temp.IDKhuyenMai == null ? temp.GiaBan : (100 - _context.KhuyenMais.First(x => x.ID == temp.IDKhuyenMai).GiaTri) / 100 * temp.GiaBan, GiaGoc = temp.GiaBan, TrangThai = temp.TrangThai, Anh = _context.Anhs.First(x => x.IDMauSac == temp.IDMauSac && x.IDSanPham == temp.IDSanPham).DuongDan };
            return chiTietSanPham;
        }
        public async Task<ChiTietSanPhamViewModelHome> GetAllChiTietSanPhamHome(Guid idSanPham)
        {
            var sanPham = await _context.SanPhams.FindAsync(idSanPham);
            List<ChiTietSanPham> lstChiTietSanPham = _context.ChiTietSanPhams.Where(x => x.IDSanPham == idSanPham).ToList();
            List<MauSac> mauSacs = new List<MauSac>();
            List<KichCo> kichCos = new List<KichCo>();
            foreach (var x in lstChiTietSanPham)
            {
                mauSacs.Add(_context.MauSacs.FindAsync(x.IDMauSac).Result);
                kichCos.Add(_context.KichCos.FindAsync(x.IDKichCo).Result);
            }
            ChiTietSanPhamViewModelHome chiTietSanPham = new ChiTietSanPhamViewModelHome();
            chiTietSanPham.Ten = sanPham.Ten;

            chiTietSanPham.Anhs = _context.Anhs.Where(x => x.IDSanPham == idSanPham).ToList(); ;
            chiTietSanPham.ChiTietSanPhams = lstChiTietSanPham;
            chiTietSanPham.MauSacs = mauSacs.Distinct().ToList();
            chiTietSanPham.KichCos = kichCos.Distinct().ToList();
            chiTietSanPham.MoTa = sanPham.MoTa;
            var query = await (from sp in _context.SanPhams.Where(p => p.ID == idSanPham)
                               join ctsp in _context.ChiTietSanPhams on sp.ID equals ctsp.IDSanPham
                               join cthd in _context.ChiTietHoaDons on ctsp.ID equals cthd.IDCTSP
                               join dg in _context.DanhGias on cthd.ID equals dg.ID
                               join hd in _context.HoaDons on cthd.IDHoaDon equals hd.ID
                               join lstd in _context.LichSuTichDiems on hd.ID equals lstd.IDHoaDon
                               join kh in _context.KhachHangs on lstd.IDKhachHang equals kh.IDKhachHang
                               join cl in _context.ChatLieus on sp.IDChatLieu equals cl.ID
                               join ms in _context.MauSacs on ctsp.IDMauSac equals ms.ID
                               join kc in _context.KichCos on ctsp.IDKichCo equals kc.ID
                               select new DanhGiaViewModel()
                               {
                                   ID = dg.ID,
                                   Sao = dg.Sao,
                                   BinhLuan = dg.BinhLuan,
                                   TrangThai = dg.TrangThai,
                                   TenKH = kh.Ten,
                                   ChatLieu = cl.Ten,
                                   MauSac = ms.Ten,
                                   KichCo = kc.Ten,
                                   NgayDanhGia = dg.NgayDanhGia
                               }).ToListAsync();
            chiTietSanPham.LSTDanhGia = query;
            foreach (var item in query)
            {
                chiTietSanPham.SoSao += Convert.ToInt32(item.Sao);
            }
            var sptt = await (from sp in _context.SanPhams.Where(p => p.IDLoaiSP == sanPham.IDLoaiSP)
                               join ctsp in _context.ChiTietSanPhams.Where(p=>p.TrangThai == 1) on sp.ID equals ctsp.IDSanPham
                               join ms in _context.MauSacs on ctsp.IDMauSac equals ms.ID
                               join anh in _context.Anhs on sp.ID equals anh.IDSanPham
                               select new SanPhamTuongTuViewModel()
                               {
                                   IDSP = sp.ID,
                                   TenSP = sp.Ten,
                                   GiaSPTT = ctsp.GiaBan,
                                   DuongDanSPTT = anh.DuongDan
                               }).ToListAsync();
            chiTietSanPham.SoSao = chiTietSanPham.SoSao / query.Count();
            chiTietSanPham.SoDanhGia = query.Count();
            chiTietSanPham.LSTSPTuongTu = sptt;
            return chiTietSanPham;
        }
        public Task<bool> UpdateChiTietSanPham(ChiTietSanPham chiTietSanPham)
        {
            throw new NotImplementedException();
        }
        public async Task<ChiTietSanPhamRequest> CreateChiTietSanPhamFromSanPham(MauSac mauSacRequest, string tenKichCo)
        {
            var mauSac = _context.MauSacs.FirstOrDefault(x => x.Ma == mauSacRequest.Ma);
            if (mauSac == null)
            {
                mauSac = new MauSac() { ID = Guid.NewGuid(), Ten = mauSacRequest.Ten, Ma = mauSacRequest.Ma.ToLower(), TrangThai = 1 };
                _context.Add(mauSac);
            }
            var kichCo = _context.KichCos.FirstOrDefault(x => x.Ten == tenKichCo);
            if (kichCo == null)
            {
                kichCo = new KichCo() { ID = Guid.NewGuid(), Ten = tenKichCo, TrangThai = 1 };
                _context.Add(kichCo);

            }
            _context.SaveChanges();
            var chiTietSanPham = new ChiTietSanPhamRequest() { IDChiTietSanPham = Guid.NewGuid(), SoLuong = 0, GiaBan = 0, IDMauSac = mauSac.ID.Value, IDKichCo = kichCo.ID, MaMau = mauSac.Ma, TenMauSac = mauSac.Ten, TenKichCo = kichCo.Ten };
            return chiTietSanPham;
        }
        public async Task<bool> AddChiTietSanPhamFromSanPham(ChiTietSanPhamUpdateRequest request)
        {
            try
            {
                var tempTrangThai = new Guid(request.TrangThai);
                foreach (var x in request.ChiTietSanPhams)
                {
                    _context.ChiTietSanPhams.Add(new ChiTietSanPham() { ID = x.IDChiTietSanPham, SoLuong = x.SoLuong, GiaBan = x.GiaBan, NgayTao = DateTime.Now, TrangThai = x.IDChiTietSanPham == tempTrangThai ? 1 : 2, IDSanPham = request.IDSanPham, IDMauSac = x.IDMauSac, IDKichCo = x.IDKichCo });
                }
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        //public AnhRequest? AddChiTietSanPham(MauSac mauSacRequest, string tenKichCo, Guid idSanPham)
        //{
        //    var temp = 0;
        //    var mauSac = _context.MauSacs.FirstOrDefault(x => x.Ma == mauSacRequest.Ma);
        //    if (mauSac == null)
        //    {
        //        mauSac = new MauSac() { ID = Guid.NewGuid(), Ten = mauSacRequest.Ten, Ma = mauSacRequest.Ma, TrangThai = 1 };
        //        _context.Add(mauSac);
        //        temp++;
        //    }
        //    var kichCo = _context.KichCos.FirstOrDefault(x => x.Ten == tenKichCo);
        //    if (kichCo == null)
        //    {
        //        kichCo = new KichCo() { ID = Guid.NewGuid(), Ten = tenKichCo, TrangThai = 1 };
        //        _context.Add(kichCo);
        //        temp++;
        //    }
        //    if (temp > 0)
        //    {
        //        var chiTietSanPham = new ChiTietSanPham() { ID = Guid.NewGuid(), SoLuong = 0, GiaBan = 0, NgayTao = DateTime.Now, TrangThai = 2, IDSanPham = idSanPham, IDMauSac = mauSac.ID.Value, IDKichCo = kichCo.ID };
        //        _context.Add(chiTietSanPham);
        //        return new AnhRequest() { IDMauSac = mauSac.ID.Value, TenMauSac = mauSac.Ten, MaMauSac = mauSac.Ma, IDSanPham = idSanPham };
        //    }
        //    else if (!_context.ChiTietSanPhams.Any(x => x.IDMauSac == mauSac.ID && x.IDKichCo == kichCo.ID && x.IDSanPham == idSanPham))
        //    {
        //        var chiTietSanPham = new ChiTietSanPham() { ID = Guid.NewGuid(), SoLuong = 0, GiaBan = 0, NgayTao = DateTime.Now, TrangThai = 2, IDSanPham = idSanPham, IDMauSac = mauSac.ID.Value, IDKichCo = kichCo.ID };
        //        _context.Add(chiTietSanPham);
        //        if (!_context.Anhs.Any(x => x.IDMauSac == mauSac.ID && x.IDSanPham == idSanPham))
        //        {
        //            return new AnhRequest() { IDMauSac = mauSac.ID.Value, TenMauSac = mauSac.Ten, MaMauSac = mauSac.Ma, IDSanPham = idSanPham };
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    return null;
        //}
        public async Task<List<ChiTietSanPhamViewModelAdmin>> GetAllChiTietSanPhamAdmin(Guid idSanPham)
        {
            try
            {
                var lstChiTietSanPham = await (from a in _context.ChiTietSanPhams.Where(x => x.IDSanPham == idSanPham)
                                               join b in _context.MauSacs on a.IDMauSac equals b.ID
                                               join c in _context.KichCos on a.IDKichCo equals c.ID
                                               select new ChiTietSanPhamViewModelAdmin()
                                               {
                                                   MaMauSac = b.Ma,
                                                   TenKichCo = c.Ten,
                                                   SoLuong = a.SoLuong,
                                                   GiaBan = a.GiaBan,
                                                   NgayTao = a.NgayTao,
                                                   TenKhuyenMai = a.IDKhuyenMai == null ? "Ko" : _context.KhuyenMais.First(x => x.ID == a.IDKhuyenMai).Ten
                                               }).ToListAsync();
                return lstChiTietSanPham;
            }
            catch
            {
                return new List<ChiTietSanPhamViewModelAdmin>();
            }
        }
        public async Task<List<ChiTietSanPhamViewModel>> GetAllChiTietSanPham()
        {
            try
            {
                return await (from sp in _context.SanPhams.AsNoTracking()
                              join ctsp in _context.ChiTietSanPhams.AsNoTracking()
                              on sp.ID equals ctsp.IDSanPham
                              select new ChiTietSanPhamViewModel()
                              {
                                  ID = ctsp.ID,
                                  Ten = sp.Ten,
                                  GiaBan = ctsp.GiaBan,
                                  GiaGoc = ctsp.GiaBan,
                                  TrangThai = ctsp.TrangThai,
                                  SoLuong = ctsp.SoLuong,
                              }).ToListAsync();
            }
            catch
            {
                return new List<ChiTietSanPhamViewModel>();
            }
        }
        public async Task<bool> DeleteChiTietSanPham(Guid id)
        {
            try
            {
                var chiTietSanPham = await _context.ChiTietSanPhams.FindAsync(id);
                chiTietSanPham.TrangThai = 0;
                return true;
            }
            catch
            {
                return false;
            }
        }
        //public AnhRequest? AddChiTietSanPham(MauSac mauSacRequest, string tenKichCo, Guid idSanPham)
        //{
        //    var temp = 0;
        //    var mauSac = _context.MauSacs.FirstOrDefault(x => x.Ma == mauSacRequest.Ma);
        //    if (mauSac == null)
        //    {
        //        mauSac = new MauSac() { ID = Guid.NewGuid(), Ten = mauSacRequest.Ten, Ma = mauSacRequest.Ma, TrangThai = 1 };
        //        _context.Add(mauSac);
        //        temp++;
        //    }
        //    var kichCo = _context.KichCos.FirstOrDefault(x => x.Ten == tenKichCo);
        //    if (kichCo == null)
        //    {
        //        kichCo = new KichCo() { ID = Guid.NewGuid(), Ten = tenKichCo, TrangThai = 1 };
        //        _context.Add(kichCo);
        //        temp++;
        //    }
        //    if (temp > 0)
        //    {
        //        var chiTietSanPham = new ChiTietSanPham() { ID = Guid.NewGuid(), SoLuong = 0, GiaBan = 0, NgayTao = DateTime.Now, TrangThai = 2, IDSanPham = idSanPham, IDMauSac = mauSac.ID.Value, IDKichCo = kichCo.ID };
        //        _context.Add(chiTietSanPham);
        //        return new AnhRequest() { IDMauSac = mauSac.ID.Value, TenMauSac = mauSac.Ten, MaMauSac = mauSac.Ma, IDSanPham = idSanPham };
        //    }
        //    else if (!_context.ChiTietSanPhams.Any(x => x.IDMauSac == mauSac.ID && x.IDKichCo == kichCo.ID && x.IDSanPham == idSanPham))
        //    {
        //        var chiTietSanPham = new ChiTietSanPham() { ID = Guid.NewGuid(), SoLuong = 0, GiaBan = 0, NgayTao = DateTime.Now, TrangThai = 2, IDSanPham = idSanPham, IDMauSac = mauSac.ID.Value, IDKichCo = kichCo.ID };
        //        _context.Add(chiTietSanPham);
        //        if (!_context.Anhs.Any(x => x.IDMauSac == mauSac.ID && x.IDSanPham == idSanPham))
        //        {
        //            return new AnhRequest() { IDMauSac = mauSac.ID.Value, TenMauSac = mauSac.Ten, MaMauSac = mauSac.Ma, IDSanPham = idSanPham };
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    return null;
        //}

        //public async Task<bool> IsExistChiTietSanPham(string maMauSac, string tenKichCo)
        //{
        //    var mauSac = await _context.MauSacs.FirstOrDefaultAsync(x => x.Ma == maMauSac);
        //    if (mauSac == null) return false;
        //    var kichCo = await _context.KichCos.FirstOrDefaultAsync(x => x.Ten == tenKichCo);
        //    if (kichCo == null) return false;
        //    return await _context.ChiTietSanPhams.AnyAsync(x => x.IDMauSac == mauSac.ID && x.IDKichCo == kichCo.ID);
        //}
        #endregion

        #region LoaiSP
        public Task<LoaiSP> SaveLoaiSP(LoaiSPRequest lsp)
        {
            throw new NotImplementedException();
        }
        public Task<LoaiSP> GetLoaiSPById(Guid id)
        {
            throw new NotImplementedException();
        }
        public bool CheckTrungLoaiSP(LoaiSPRequest lsp)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> DeleteLoaiSP(Guid id)
        {
            try
            {
                var loaiSP = await _context.LoaiSPs.FindAsync(id);
                loaiSP.TrangThai = 0;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<List<LoaiSP>> GetAllLoaiSPCha()
        {
            return await _context.LoaiSPs.Where(x => x.IDLoaiSPCha == null).ToListAsync();
        }
        public async Task<List<LoaiSP>> GetAllLoaiSPCon(string tenLoaiSPCha)
        {
            var loaiSPCha = await _context.LoaiSPs.Where(x => x.IDLoaiSPCha == null).FirstAsync(x => x.Ten == tenLoaiSPCha);
            return await _context.LoaiSPs.Where(x => x.IDLoaiSPCha == loaiSPCha.ID).ToListAsync();
        }
        #endregion

        #region Other
        public async Task<List<MauSac>> GetAllMauSac()
        {
            return await _context.MauSacs.ToListAsync();
        }

        public async Task<List<KichCo>> GetAllKichCo()
        {
            return await _context.KichCos.ToListAsync();
        }
        public async Task<List<ChatLieu>> GetAllChatLieu()
        {
            return await _context.ChatLieus.ToListAsync();
        }

        public Task<List<ChiTietSanPham>> GetAllChiTietSanPham(Guid idSanPham)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SanPham
        //TÌM KIẾM NÂNG CAO : Tên, List Loại Sp, khoảng Giá
        //public async Task<List<SanPhamViewModel>> TimKiemSanPham(SanPhamTimKiemNangCao sptk)
        //{
        //    // Lấy ds Sản phẩm cùng loại sp của chúng
        //    var sanphams = (from sp in _context.SanPhams.AsNoTracking()
        //                    join bt in _context.BienThes.AsNoTracking() on sp.ID equals bt.IDSanPham
        //                    join lsp in _context.LoaiSPs.AsNoTracking() on sp.IDLoaiSP equals lsp.ID
        //                    select new { sp, bt, lsp });
        //    // Tìm tên
        //    if (!string.IsNullOrEmpty(sptk.KeyWord))
        //    {
        //        sanphams = sanphams.AsNoTracking().Where(c => c.sp.Ten.ToLower().Contains(sptk.KeyWord.ToLower()));
        //    }
        //    // Loại sp
        //    if (sptk.IdLoaiSP.Count() > 0)
        //    {
        //        sanphams = sanphams.AsNoTracking().Where(c => sptk.IdLoaiSP.Contains(c.sp.IDLoaiSP));
        //    }
        //    // Khoảng giá
        //    if (sptk.GiaMax != 0 && sptk.GiaMin != 0)
        //    {
        //        sanphams = sanphams.AsNoTracking().Where(c => c.bt.GiaBan >= sptk.GiaMin && c.bt.GiaBan <= sptk.GiaMax);
        //    }
        //    var result = await sanphams.AsNoTracking().Select(c => new SanPhamViewModel()
        //    {
        //        ID = c.sp.ID,
        //        Ten = c.sp.Ten,
        //        TrangThai = c.sp.TrangThai,
        //        LoaiSP = c.lsp.Ten,
        //        IdBT = c.bt.ID,
        //        Image = (from img in _context.Anhs.AsNoTracking()
        //                 join abt in _context.AnhBienThes.AsNoTracking()
        //                 on img.ID equals abt.IdAnh
        //                 where abt.IdBienThe == c.bt.ID
        //                 select img.Ten).FirstOrDefault(),
        //        GiaGoc = c.bt.GiaBan,
        //        GiaBan = c.bt.IDKhuyenMai == null ? c.bt.GiaBan : (from km in _context.KhuyenMais.AsNoTracking()
        //                                                           where km.ID == c.bt.IDKhuyenMai
        //                                                           select c.bt.GiaBan * (100 - km.GiaTri) / 100).FirstOrDefault()
        //    }).ToListAsync();
        //    return result;
        //}
        ////CHECK TRÙNG TÊN SP
        //public bool CheckTrungTenSP(SanPhamRequest lsp)
        //{
        //    if (!_context.SanPhams.AsNoTracking().Any(c => c.Ten == lsp.Ten && c.ID != lsp.ID))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //// LẤY DS SẢN PHẨM
        //public async Task<List<SanPhamViewModel>> GetAllSanPham()
        //{
        //    var sanphams = await (from sp in _context.SanPhams.AsNoTracking()
        //                          join lsp in _context.LoaiSPs.AsNoTracking() on sp.IDLoaiSP equals lsp.ID
        //                          join bt in _context.BienThes.AsNoTracking() on sp.ID equals bt.IDSanPham
        //                          where sp.TrangThai == 1 && bt.IsDefault == true// Sản phẩm hoạt động
        //                          select new SanPhamViewModel()
        //                          {
        //                              ID = sp.ID,
        //                              Ten = sp.Ten,
        //                              TrangThai = sp.TrangThai,
        //                              LoaiSP = lsp.Ten,
        //                              IdBT = bt.ID,
        //                              Image = (from img in _context.Anhs.AsNoTracking()
        //                                       join abt in _context.AnhBienThes.AsNoTracking()
        //                                       on img.ID equals abt.IdAnh
        //                                       where abt.IdBienThe == bt.ID
        //                                       select img.Ten).FirstOrDefault(),
        //                              GiaGoc = bt.GiaBan,
        //                              GiaBan = bt.IDKhuyenMai == null ? bt.GiaBan : (from km in _context.KhuyenMais.AsNoTracking()
        //                                                                             where km.ID == bt.IDKhuyenMai
        //                                                                             select bt.GiaBan * (100 - km.GiaTri) / 100).FirstOrDefault()
        //                          }).ToListAsync();
        //    return sanphams;
        //}
        //// Lấy DS sản phẩm theo IdLsp
        //public async Task<List<SanPhamViewModel>> GetSanPhamByIdDanhMuc(Guid idloaisp)
        //{
        //    var lsp = await _context.LoaiSPs.FindAsync(idloaisp);
        //    if (lsp.IDLoaiSPCha == null) // Danh mục cha -> Lấy sp của toàn bộ danh mục con
        //    {
        //        // Loại sp con
        //        var lspc = await _context.LoaiSPs.AsNoTracking().Where(c => c.IDLoaiSPCha == lsp.ID).Select(c => c.ID).ToListAsync();

        //        var result = await (from sp in _context.SanPhams.AsNoTracking()
        //                            join cate in _context.LoaiSPs.AsNoTracking() on sp.IDLoaiSP equals cate.ID
        //                            join bt in _context.BienThes.AsNoTracking() on sp.ID equals bt.IDSanPham
        //                            where sp.TrangThai == 1 && bt.IsDefault == true
        //                                   && lspc.Contains(sp.IDLoaiSP)
        //                            select new SanPhamViewModel()
        //                            {
        //                                ID = sp.ID,
        //                                Ten = sp.Ten,
        //                                TrangThai = sp.TrangThai,
        //                                LoaiSP = cate.Ten,
        //                                IdBT = bt.ID,
        //                                Image = (from img in _context.Anhs.AsNoTracking()
        //                                         join abt in _context.AnhBienThes.AsNoTracking() on img.ID equals abt.IdAnh
        //                                         where abt.IdBienThe == bt.ID
        //                                         select img.Ten).FirstOrDefault(),
        //                                GiaGoc = bt.GiaBan,
        //                                GiaBan = bt.IDKhuyenMai == null ? bt.GiaBan : (from km in _context.KhuyenMais.AsNoTracking()
        //                                                                               where km.ID == bt.IDKhuyenMai
        //                                                                               select bt.GiaBan * (100 - km.GiaTri) / 100).FirstOrDefault()
        //                            }).ToListAsync();
        //        return result;
        //    }
        //    else
        //    {
        //        var result = await (from sp in _context.SanPhams.AsNoTracking()
        //                            join loaisp in _context.LoaiSPs.AsNoTracking() on sp.IDLoaiSP equals lsp.ID
        //                            join bt in _context.BienThes.AsNoTracking() on sp.ID equals bt.IDSanPham
        //                            where sp.TrangThai == 1 && bt.IsDefault == true// Sản phẩm hoạt động
        //                            && loaisp.ID == idloaisp
        //                            select new SanPhamViewModel()
        //                            {
        //                                ID = sp.ID,
        //                                Ten = sp.Ten,
        //                                TrangThai = sp.TrangThai,
        //                                LoaiSP = lsp.Ten,
        //                                IdBT = bt.ID,
        //                                Image = (from img in _context.Anhs.AsNoTracking()
        //                                         join abt in _context.AnhBienThes.AsNoTracking()
        //                                         on img.ID equals abt.IdAnh
        //                                         where abt.IdBienThe == bt.ID
        //                                         select img.Ten).FirstOrDefault(),
        //                                GiaGoc = bt.GiaBan,
        //                                GiaBan = bt.IDKhuyenMai == null ? bt.GiaBan : (from km in _context.KhuyenMais.AsNoTracking()
        //                                                                               where km.ID == bt.IDKhuyenMai
        //                                                                               select bt.GiaBan * (100 - km.GiaTri) / 100).FirstOrDefault()
        //                            }).ToListAsync();
        //        return result;
        //    }
        //}
        //// Hiển thị Sản phẩm và Biến thể qua IdSP
        //public async Task<SanPhamDetail> GetSanPhamById(Guid idsp)
        //{
        //    var sanpham = await (from sp in _context.SanPhams.AsNoTracking()
        //                         join lsp in _context.LoaiSPs.AsNoTracking() on sp.IDLoaiSP equals lsp.ID
        //                         where sp.ID == idsp
        //                         select new SanPhamDetail()
        //                         {
        //                             ID = sp.ID,
        //                             Ten = sp.Ten,
        //                             MoTa = sp.MoTa,
        //                             TrangThai = sp.TrangThai,
        //                             LoaiSP = lsp.Ten,
        //                             ThuocTinhs = (from tt in _context.ThuocTinhs.AsNoTracking()
        //                                           join ttsp in _context.ThuocTinhSanPhams.AsNoTracking()
        //                                           on tt.ID equals ttsp.IDThuocTinh
        //                                           where ttsp.IDSanPham == sp.ID
        //                                           select new ThuocTinhRequest()
        //                                           {
        //                                               ID = tt.ID,
        //                                               Ten = tt.Ten,
        //                                               GiaTriRequests = (from gt in _context.GiaTris.AsNoTracking()
        //                                                                 where gt.IdThuocTinh == tt.ID
        //                                                                 select new GiaTriRequest()
        //                                                                 {
        //                                                                     ID = gt.ID,
        //                                                                     Ten = gt.Ten
        //                                                                 }).ToList(),
        //                                           }).ToList(),
        //                             BienThes = (from bt in _context.BienThes.AsNoTracking()
        //                                         where bt.IDSanPham == sp.ID
        //                                         select new BienTheViewModel()
        //                                         {
        //                                             ID = bt.ID,
        //                                             Ten = sp.Ten,
        //                                             SoLuong = bt.SoLuong,
        //                                             GiaGoc = bt.GiaBan,
        //                                             GiaBan = bt.IDKhuyenMai == null ? bt.GiaBan : (from km in _context.KhuyenMais where bt.IDKhuyenMai == km.ID select ((100 - km.GiaTri) / 100) * bt.GiaBan).FirstOrDefault(),
        //                                             TrangThai = bt.TrangThai,
        //                                             Anh = (from img in _context.Anhs.AsNoTracking() join btimg in _context.AnhBienThes on img.ID equals btimg.IdAnh where btimg.IdBienThe == bt.ID select img.Ten).ToList(),
        //                                             GiaTris = (from gt in _context.GiaTris.AsNoTracking()
        //                                                        join ctbt in _context.ChiTietBienThes on gt.ID equals ctbt.IDGiaTri
        //                                                        where ctbt.IDBienThe == bt.ID
        //                                                        select new GiaTriRequest()
        //                                                        {
        //                                                            ID = gt.ID,
        //                                                            Ten = gt.Ten
        //                                                        }).ToList()
        //                                         }).ToList()
        //                         }).FirstOrDefaultAsync();
        //    return sanpham;
        //}
        //// TẠO HOẶC CẬP NHẬT SẢN PHẨM
        //public async Task<SanPham> SaveSanPham(SanPhamRequest request)
        //{
        //    //Kiểm tra tồn tại
        //    var sanpham = await _context.SanPhams.FindAsync(request.ID);
        //    if (sanpham != null)//Update
        //    {
        //        sanpham.MoTa = request.MoTa;
        //        sanpham.Ten = request.Ten;
        //        sanpham.IDLoaiSP = request.IDLoaiSP;
        //        sanpham.TrangThai = request.TrangThai;
        //        _context.SanPhams.Update(sanpham);
        //        await _context.SaveChangesAsync();

        //        //Thuộc tính
        //        foreach (var id in request.ListIdThuocTinh)
        //        {
        //            // Check SP đã có TT này chưa
        //            var ttspExist = await _context.ThuocTinhSanPhams.AsNoTracking().FirstOrDefaultAsync(c => c.IDSanPham == sanpham.ID && c.IDThuocTinh == id);
        //            if (ttspExist == null) // Chưa có -> tạo SP-TT
        //            {
        //                ThuocTinhSanPham ttsp = new ThuocTinhSanPham()
        //                {
        //                    ID = new Guid(),
        //                    IDSanPham = sanpham.ID,
        //                    IDThuocTinh = id,
        //                    NgayLuu = DateTime.Now,
        //                };
        //                await _context.ThuocTinhSanPhams.AddAsync(ttsp);
        //                await _context.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                ttspExist.NgayLuu = DateTime.Now;
        //                _context.ThuocTinhSanPhams.Update(ttspExist);
        //                await _context.SaveChangesAsync();
        //            }
        //        }
        //        // Xóa TT-SP mà sản phẩm không còn nữa
        //        var idTTSPSD = await _context.ThuocTinhSanPhams.AsNoTracking().OrderByDescending(c => c.NgayLuu).Take(request.ListIdThuocTinh.Count).Select(c => c.ID).ToListAsync();
        //        var lstdelete = await _context.ThuocTinhSanPhams.AsNoTracking().Where(c => !idTTSPSD.Contains(c.ID) && c.IDSanPham == sanpham.ID).ToListAsync();
        //        _context.ThuocTinhSanPhams.RemoveRange(lstdelete);
        //        await _context.SaveChangesAsync();
        //        return sanpham;
        //    }
        //    else // Ngược lại ->Tạo mới
        //    {
        //        SanPham sp = new SanPham()
        //        {
        //            ID = new Guid(),
        //            Ten = request.Ten,
        //            MoTa = request.MoTa,
        //            TrangThai = request.TrangThai,
        //            IDLoaiSP = request.IDLoaiSP,
        //        };
        //        await _context.SanPhams.AddAsync(sp);
        //        await _context.SaveChangesAsync();

        //        foreach (var id in request.ListIdThuocTinh)
        //        {
        //            var thuoctinh = await _context.ThuocTinhs.FindAsync(id);
        //            ThuocTinhSanPham ttsp = new ThuocTinhSanPham()
        //            {
        //                IDSanPham = sp.ID,
        //                IDThuocTinh = id,
        //                NgayLuu = DateTime.Now,
        //            };
        //            await _context.ThuocTinhSanPhams.AddAsync(ttsp);
        //            await _context.SaveChangesAsync();
        //        }
        //        return sp;
        //    }
        //}
        //// XÓA SẢN PHẨM
        //public async Task<bool> DeleteSanPham(Guid id)
        //{
        //    try
        //    {
        //        var sp = await _context.SanPhams.FindAsync(id);
        //        // Xóa TT-SP
        //        var listTTSP = await _context.ThuocTinhSanPhams.AsNoTracking().Where(c => c.IDSanPham == id).ToListAsync();
        //        _context.ThuocTinhSanPhams.RemoveRange(listTTSP);
        //        await _context.SaveChangesAsync();
        //        // Xóa Biến thể : Xóa CTBT, Ảnh -> Xóa BT
        //        var listBT = await _context.BienThes.AsNoTracking().Where(c => c.IDSanPham == id).ToListAsync();
        //        foreach (var bt in listBT)
        //        {
        //            //Xóa CTBT
        //            var listCTBT = await _context.ChiTietBienThes.AsNoTracking().Where(c => c.IDBienThe == bt.ID).ToListAsync();
        //            _context.ChiTietBienThes.RemoveRange(listCTBT);
        //            await _context.SaveChangesAsync();
        //            //Xóa Ảnh
        //            var listanhbt = await _context.AnhBienThes.AsNoTracking().Where(c => c.IdBienThe == bt.ID).ToListAsync();
        //            _context.AnhBienThes.RemoveRange(listanhbt);
        //        }
        //        _context.BienThes.RemoveRange(listBT);
        //        await _context.SaveChangesAsync();

        //        _context.SanPhams.Remove(sp);
        //        await _context.SaveChangesAsync();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        #endregion

        //Nhinh thêm
        #region SanPhamBanHang
        public async Task<List<SanPhamBanHang>> GetAllSanPhamTaiQuay()
        {
            return await (from sp in _context.SanPhams.AsNoTracking()
                          let anh = _context.Anhs.AsNoTracking().Where(c => c.IDSanPham == sp.ID && c.DuongDan != null).FirstOrDefault()
                          let gia = _context.ChiTietSanPhams.AsNoTracking().Where(x => x.IDSanPham == sp.ID && x.GiaBan != null).FirstOrDefault()
                          select new SanPhamBanHang()
                          {
                              Id = sp.ID,
                              Ten = sp.Ten,
                              Anh = anh != null ? anh.DuongDan : null,
                              GiaBan = gia != null ? gia.GiaBan : 0,
                          }).ToListAsync();
        }

        public async Task<ChiTietSanPhamBanHang> GetChiTietSPBHById(Guid idsp)
        {
            var lstMS = (from ctsp in _context.ChiTietSanPhams.AsNoTracking()
                         join ms in _context.MauSacs.AsNoTracking() on ctsp.IDMauSac equals ms.ID
                         where ctsp.IDSanPham == idsp
                         select new MauSac
                         {
                             ID = ms.ID,
                             Ma = ms.Ma,
                             Ten = ms.Ten,
                         }).Distinct().ToList();

            var lstKC = (from ctsp in _context.ChiTietSanPhams
                         join kc in _context.KichCos on ctsp.IDKichCo equals kc.ID
                         where ctsp.IDSanPham == idsp
                         select new KichCo
                         {
                             ID = kc.ID,
                             Ten = kc.Ten,
                         }).Distinct().ToList();

            var result = await (from sp in _context.SanPhams.AsNoTracking()
                                where sp.ID == idsp
                                select new ChiTietSanPhamBanHang()
                                {
                                    Id = sp.ID,
                                    Ten = sp.Ten,
                                    lstMau = lstMS,
                                    lstKC = lstKC,
                                }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<ChiTietCTSPBanHang>> GetChiTietCTSPBanHang(Guid idsp)
        {
            return await (from ctsp in _context.ChiTietSanPhams
                          join ms in _context.MauSacs on ctsp.IDMauSac equals ms.ID
                          join kc in _context.KichCos on ctsp.IDKichCo equals kc.ID
                          join sp in _context.SanPhams on ctsp.IDSanPham equals sp.ID
                          join km in _context.KhuyenMais on ctsp.IDKhuyenMai equals km.ID
                          into kmGroup
                          from km in kmGroup.DefaultIfEmpty()
                          where ctsp.IDSanPham == idsp
                          select new ChiTietCTSPBanHang()
                          {
                              Id = ctsp.ID,
                              Ten = sp.Ten,
                              ChiTiet = ms.Ten + " - " + kc.Ten,
                              idMauSac = ctsp.IDMauSac,
                              idKichCo = ctsp.IDKichCo,
                              SoLuong = ctsp.SoLuong,
                              Anh = (from ms in _context.MauSacs
                                     join a in _context.Anhs on ms.ID equals a.IDMauSac
                                     where ms.ID == ctsp.IDMauSac && a.IDSanPham == ctsp.IDSanPham
                                     select a).FirstOrDefault().DuongDan,
                              GiaGoc = ctsp.GiaBan,
                              GiaBan = km.TrangThai == null ? ctsp.GiaBan : (km.TrangThai == 1 ? ctsp.GiaBan - km.GiaTri : (ctsp.GiaBan * (100 - km.GiaTri) / 100)),
                          }).ToListAsync();
        }
        #endregion
    }
}
