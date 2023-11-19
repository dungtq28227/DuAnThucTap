using AppAPI.IServices;
using AppData.IRepositories;
using AppData.Models;
using AppData.Repositories;
using AppData.ViewModels;
using AppData.ViewModels.BanOffline;
using System.Security.Cryptography.Xml;

namespace AppAPI.Services
{
    public class HoaDonService : IHoaDonService
    {
        private readonly IAllRepository<HoaDon> reposHoaDon;
        private readonly IAllRepository<ChiTietHoaDon> reposChiTietHoaDon;
        private readonly IAllRepository<ChiTietSanPham> repsCTSanPham;
        private readonly IAllRepository<Voucher> reposVoucher;
        private readonly IAllRepository<QuyDoiDiem> reposQuyDoiDiem;
        private readonly IAllRepository<LichSuTichDiem> reposLichSuTichDiem;
        private readonly IAllRepository<KhachHang> reposKhachHang;
        private readonly IAllRepository<SanPham> reposSanPham;
        //private readonly IAllRepository<PhuongThucThanhToan> reposPTTT;
        //private readonly IAllRepository<ChiTietPTTT> reposChiTietPTTT;
        private readonly IAllRepository<DanhGia> reposDanhGia;
        private readonly IAllRepository<NhanVien> reposNhanVien;


        AssignmentDBContext context = new AssignmentDBContext();

        public HoaDonService()
        {
            reposHoaDon = new AllRepository<HoaDon>(context, context.HoaDons);
            reposChiTietHoaDon = new AllRepository<ChiTietHoaDon>(context, context.ChiTietHoaDons);
            repsCTSanPham = new AllRepository<ChiTietSanPham>(context, context.ChiTietSanPhams);
            reposVoucher = new AllRepository<Voucher>(context, context.Vouchers);
            reposQuyDoiDiem = new AllRepository<QuyDoiDiem>(context, context.QuyDoiDiems);
            reposLichSuTichDiem = new AllRepository<LichSuTichDiem>(context, context.LichSuTichDiems);
            reposKhachHang = new AllRepository<KhachHang>(context, context.KhachHangs);
            reposSanPham = new AllRepository<SanPham>(context, context.SanPhams);
            //reposPTTT = new AllRepository<PhuongThucThanhToan>(context, context.PhuongThucThanhToans);
            reposDanhGia = new AllRepository<DanhGia>(context, context.DanhGias);
            //reposChiTietPTTT = new AllRepository<ChiTietPTTT>(context, context.ChiTietPTTTs);
            context = new AssignmentDBContext();
        }

        public bool CheckHDHasLSGD(Guid idHoaDon)
        {
            var exist = reposLichSuTichDiem.GetAll().Any(c => c.IDHoaDon == idHoaDon);
            if (exist == true)
            {
                return true;
            }
            return false;
        }

        public int CheckVoucher(string ten, int tongtien)
        {
            var voucher = reposVoucher.GetAll().FirstOrDefault(p => p.Ten == ten);
            if (voucher != null)
            {
                if (tongtien >= voucher.SoTienCan && DateTime.Compare(voucher.NgayApDung, DateTime.Now) <= 0 && DateTime.Compare(DateTime.Now, voucher.NgayKetThuc) <= 0 && voucher.SoLuong > 0)
                {
                    if (voucher.HinhThucGiamGia == 1)
                    {
                        tongtien -= voucher.GiaTri;
                        return tongtien;
                    }
                    else
                    {
                        tongtien = tongtien - (tongtien * voucher.GiaTri / 100);
                        return tongtien;
                    }

                }
                else
                {
                    return tongtien;
                }
            }
            else
            {
                return tongtien;
            }
        }

        //Thanh toan online
        public bool CreateHoaDon(List<ChiTietHoaDonViewModel> chiTietHoaDons, HoaDonViewModel hoaDon)
        {
            try
            {
                var voucher = reposVoucher.GetAll().FirstOrDefault(p => p.Ten == hoaDon.TenVoucher);
                if (chiTietHoaDons != null)
                {
                    HoaDon hoaDon1 = new HoaDon();
                    hoaDon1.ID = Guid.NewGuid();
                    hoaDon1.IDNhanVien = null;
                    if (voucher != null)
                    {
                        hoaDon1.IDVoucher = voucher.ID;
                        voucher.SoLuong--;
                        reposVoucher.Update(voucher);
                    }
                    else
                    {
                        hoaDon1.IDVoucher = null;
                    }
                    hoaDon1.TenNguoiNhan = hoaDon.Ten;
                    hoaDon1.MaHD = "HD" + (hoaDon1.ID).ToString().Substring(0, 8).ToUpper();
                    hoaDon1.SDT = hoaDon.SDT;
                    hoaDon1.Email = hoaDon.Email;
                    hoaDon1.NgayTao = DateTime.Now;
                    hoaDon1.DiaChi = hoaDon.DiaChi;
                    hoaDon1.TienShip = hoaDon.TienShip;
                    hoaDon1.PhuongThucThanhToan = hoaDon.PhuongThucThanhToan;
                    hoaDon1.TrangThaiGiaoHang = 2;
                    if (reposHoaDon.Add(hoaDon1))
                    {
                        foreach (var x in chiTietHoaDons)
                        {
                            ChiTietHoaDon chiTietHoaDon = new ChiTietHoaDon();
                            chiTietHoaDon.ID = Guid.NewGuid();
                            chiTietHoaDon.IDHoaDon = hoaDon1.ID;
                            chiTietHoaDon.IDCTSP = x.IDChiTietSanPham;
                            chiTietHoaDon.SoLuong = x.SoLuong;
                            chiTietHoaDon.DonGia = x.DonGia;
                            chiTietHoaDon.TrangThai = 1;
                            DanhGia danhGia = new DanhGia();
                            danhGia.ID = chiTietHoaDon.ID;
                            danhGia.Sao = null;
                            danhGia.BinhLuan = null;
                            danhGia.TrangThai = 0;
                            reposDanhGia.Add(danhGia);
                            reposChiTietHoaDon.Add(chiTietHoaDon);
                            var CTsanPham = repsCTSanPham.GetAll().FirstOrDefault(p => p.ID == x.IDChiTietSanPham);
                            CTsanPham.SoLuong -= chiTietHoaDon.SoLuong;
                            repsCTSanPham.Update(CTsanPham);

                        }
                        //tích điểm, dùng điểm
                        if (hoaDon.IDNguoiDung != null)
                        {
                            QuyDoiDiem quyDoiDiem = reposQuyDoiDiem.GetAll().First();
                            KhachHang khachHang = reposKhachHang.GetAll().FirstOrDefault(p => p.IDKhachHang == hoaDon.IDNguoiDung);
                            if (hoaDon.Diem == 0)
                            {
                                khachHang.DiemTich += hoaDon.TongTien / quyDoiDiem.TiLeTichDiem;
                                reposKhachHang.Update(khachHang);
                                LichSuTichDiem lichSuTichDiem = new LichSuTichDiem()
                                {
                                    ID = Guid.NewGuid(),
                                    IDKhachHang = hoaDon.IDNguoiDung,
                                    IDQuyDoiDiem = quyDoiDiem.ID,
                                    IDHoaDon = hoaDon1.ID,
                                    Diem = hoaDon.TongTien / quyDoiDiem.TiLeTichDiem,
                                    TrangThai = 1
                                };
                                reposLichSuTichDiem.Add(lichSuTichDiem);
                            }
                            //dùng điểm
                            else
                            {
                                if (khachHang.DiemTich >= hoaDon.Diem)
                                {
                                    khachHang.DiemTich = khachHang.DiemTich - hoaDon.Diem;
                                    reposKhachHang.Update(khachHang);
                                    LichSuTichDiem lichSuTichDiem = new LichSuTichDiem()
                                    {
                                        ID = Guid.NewGuid(),
                                        IDKhachHang = hoaDon.IDNguoiDung,
                                        IDQuyDoiDiem = quyDoiDiem.ID,
                                        IDHoaDon = hoaDon1.ID,
                                        Diem = hoaDon.Diem.Value,
                                        TrangThai = 0
                                    };
                                    reposLichSuTichDiem.Add(lichSuTichDiem);
                                }
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        //Bán hàng tại quầy
        public bool CreateHoaDonOffline(Guid idnhanvien)
        {
            try
            {
                HoaDon hoaDon1 = new HoaDon();
                hoaDon1.ID = Guid.NewGuid();
                hoaDon1.IDNhanVien = idnhanvien;
                hoaDon1.NgayTao = DateTime.Now;
                hoaDon1.TrangThaiGiaoHang = 1;
                hoaDon1.LoaiHD = 1;
                hoaDon1.MaHD = "HD" + (hoaDon1.ID).ToString().Substring(0, 8).ToUpper();
                if (reposHoaDon.Add(hoaDon1))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        //public bool CreatePTTT(PhuongThucThanhToan pttt)
        //{
        //    try
        //    {
        //        PhuongThucThanhToan phuongTTT = new PhuongThucThanhToan()
        //        {
        //            ID = new Guid(),
        //            Ten = pttt.Ten,
        //            TrangThai = pttt.TrangThai,
        //        };
        //        reposPTTT.Add(phuongTTT);
        //        return true;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public bool DeleteHoaDon(Guid id)
        {
            try
            {
                HoaDon hoaDon = reposHoaDon.GetAll().FirstOrDefault(p => p.ID == id);
                var lsthdct = reposChiTietHoaDon.GetAll().Where(c => c.IDHoaDon == hoaDon.ID).ToList();
                
                var deletedg = context.DanhGias.Where(c => lsthdct.Select(x => x.ID).Contains(c.ID)).ToList();
                foreach (var item in lsthdct)
                {
                    var ctsp = repsCTSanPham.GetAll().FirstOrDefault(c => c.ID == item.IDCTSP);
                    ctsp.SoLuong += item.SoLuong;
                    repsCTSanPham.Update(ctsp);
                }
                //Xóa chiTietHD
                context.ChiTietHoaDons.RemoveRange(lsthdct);
                context.SaveChanges();
                //Xóa đánh giá
                context.DanhGias.RemoveRange(deletedg);
                context.SaveChanges();
                //Xóa hóa đơn
                context.HoaDons.Remove(hoaDon);
                context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        //public bool DeletePTTT(Guid id)
        //{
        //    try
        //    {
        //        var pttt = reposPTTT.GetAll().FirstOrDefault(c => c.ID == id);
        //        reposPTTT.Delete(pttt);
        //        return true;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message, ex);
        //    }
        //}

        public List<ChiTietHoaDon> GetAllChiTietHoaDon(Guid idHoaDon)
        {
            return reposChiTietHoaDon.GetAll().Where(x => x.IDHoaDon == idHoaDon).ToList();
        }

        public List<HoaDon> GetAllHDCho()
        {
            return context.HoaDons.Where(c => c.TrangThaiGiaoHang == 1).OrderBy(c => c.NgayTao).ToList();
        }
        //Nhinh
        public List<HoaDonQL> GetAllHDQly()
        {
            var result = (from hd in reposHoaDon.GetAll()
                          join lstd in reposLichSuTichDiem.GetAll() on hd.ID equals lstd?.IDHoaDon into lstdGroup
                          from lstd in lstdGroup.DefaultIfEmpty()
                          join kh in reposKhachHang.GetAll() on lstd?.IDKhachHang equals kh?.IDKhachHang into khGroup
                          from kh in khGroup.DefaultIfEmpty()
                          where hd.TrangThaiGiaoHang != 1
                          select new HoaDonQL()
                          {
                              Id = hd.ID,
                              MaHD = hd.MaHD,
                              KhachHang = kh?.Ten == null ? "Khách lẻ" : kh.Ten,
                              ThoiGian = hd.NgayThanhToan,
                              KhachTra = hd.TongTien,
                              LoaiHD = hd.LoaiHD,
                              TrangThai = hd.TrangThaiGiaoHang,
                          }).ToList();
            return result;
        }

        public List<HoaDon> GetAllHoaDon()
        {
            return reposHoaDon.GetAll();
        }

        //public List<PhuongThucThanhToan> GetAllPTTT()
        //{
        //    return reposPTTT.GetAll();
        //}

        public ChiTietHoaDonQL GetCTHDByID(Guid idhd)
        {
            var result = (from hd in context.HoaDons
                          join nv in context.NhanViens on hd.IDNhanVien equals nv.ID
                          join lstd in context.LichSuTichDiems on hd.ID equals lstd.IDHoaDon into lstdGroup
                          from lstd in lstdGroup.DefaultIfEmpty()
                          join kh in context.KhachHangs on lstd.IDKhachHang equals kh.IDKhachHang into khGroup
                          from kh in khGroup.DefaultIfEmpty()
                          where hd.ID == idhd
                          select new ChiTietHoaDonQL()
                          {
                              Id = hd.ID,
                              MaHD = hd.MaHD,
                              NgayTao = hd.NgayTao,
                              NgayThanhToan = hd.NgayThanhToan,
                              NhanVien = nv.Ten,
                              KhachHang = kh.Ten == null ? "Khách lẻ" : kh.Ten,
                              ThueVAT = hd.ThueVAT,
                              TienKhachTra = hd.TongTien,
                              voucher = (from vc in context.Vouchers
                                         where vc.ID == hd.IDVoucher
                                         select new Voucher()
                                         {
                                             ID = vc.ID,
                                             Ten = vc.Ten,
                                             GiaTri = vc.GiaTri,
                                             TrangThai = vc.TrangThai,
                                         }).FirstOrDefault(),
                              listsp = (from cthd in context.ChiTietHoaDons
                                        join ctsp in context.ChiTietSanPhams
                                        on cthd.IDCTSP equals ctsp.ID
                                        join sp in context.SanPhams on ctsp.IDSanPham equals sp.ID
                                        where cthd.IDHoaDon == idhd
                                        select new HoaDonChiTietViewModel()
                                        {
                                            Id = cthd.ID,
                                            IdHoaDon = hd.ID,
                                            IDChiTietSanPham = cthd.IDCTSP,
                                            Ten = sp.Ten,
                                            SoLuong = cthd.SoLuong,
                                            GiaGoc = ctsp.GiaBan,
                                            GiaLuu = cthd.DonGia,
                                        }).ToList(),

                          }
                          ).FirstOrDefault();
            return result;
        }

        public HoaDonViewModelBanHang GetHDBanHang(Guid id)
        {
            List<HoaDonChiTietViewModel> lsthdct = (from cthd in context.ChiTietHoaDons
                                                    join ctsp in context.ChiTietSanPhams on cthd.IDCTSP equals ctsp.ID
                                                    join ms in context.MauSacs on ctsp.IDMauSac equals ms.ID
                                                    join kc in context.KichCos on ctsp.IDKichCo equals kc.ID
                                                    join sp in context.SanPhams on ctsp.IDSanPham equals sp.ID
                                                    join km in context.KhuyenMais on ctsp.IDKhuyenMai equals km.ID
                                                    into kmGroup
                                                    from km in kmGroup.DefaultIfEmpty()
                                                    where cthd.IDHoaDon == id
                                                    select new HoaDonChiTietViewModel()
                                                    {
                                                        Id = cthd.ID,
                                                        IdHoaDon = cthd.IDHoaDon,
                                                        IdSP = sp.ID,
                                                        Ten = sp.Ten,
                                                        PhanLoai = ms.Ten + " - " + kc.Ten,
                                                        SoLuong = cthd.SoLuong,
                                                        GiaGoc = ctsp.GiaBan,
                                                        GiaKM = km.TrangThai == null ? ctsp.GiaBan : (km.TrangThai == 1 ? ctsp.GiaBan - km.GiaTri : (ctsp.GiaBan * (100 - km.GiaTri) / 100)),
                                                    }).AsEnumerable().Reverse().ToList();
            var result = (from hd in reposHoaDon.GetAll()
                          join lstd in reposLichSuTichDiem.GetAll() on hd.ID equals lstd.IDHoaDon into lstdGroup
                          from lstd in lstdGroup.DefaultIfEmpty()
                          join kh in reposKhachHang.GetAll() on lstd?.IDKhachHang equals kh?.IDKhachHang into khGroup
                          from kh in khGroup.DefaultIfEmpty()
                          where hd.ID == id
                          select new HoaDonViewModelBanHang()
                          {
                              Id = hd.ID,
                              MaHD = hd.MaHD,
                              IdKhachHang = kh?.IDKhachHang,
                              TenKhachHang = kh?.Ten,
                              lstHDCT = lsthdct,
                              GhiChu = hd.GhiChu == null ? "":hd.GhiChu,
                          }).FirstOrDefault();
            return result;
        }

        public HoaDon GetHoaDonById(Guid idhd)
        {
            return reposHoaDon.GetAll().FirstOrDefault(c => c.ID == idhd);
        }

        public LichSuTichDiem GetLichSuGiaoDichByIdHD(Guid idHoaDon)
        {
            return reposLichSuTichDiem.GetAll().FirstOrDefault(c => c.IDHoaDon == idHoaDon);
        }

        public List<HoaDon> LichSuGiaoDich(Guid idNguoiDung)
        {
            var idhoadon = reposLichSuTichDiem.GetAll().Where(p => p.IDKhachHang == idNguoiDung).ToList();
            List<HoaDon> lichSuGiaoDich = new List<HoaDon>();
            foreach (var item in idhoadon)
            {
                lichSuGiaoDich.Add(reposHoaDon.GetAll().FirstOrDefault(p => p.ID == item.IDHoaDon));
            }
            return lichSuGiaoDich;
        }

        public List<HoaDon> TimKiemVaLocHoaDon(string ten, int? loc)
        {
            List<HoaDon> timkiem = reposHoaDon.GetAll().Where(p => p.TenNguoiNhan.ToLower().Contains(ten.ToLower())).ToList();
            if (loc == 0)
            {
                List<HoaDon> locTangDan = timkiem.OrderBy(p => p.NgayTao).ToList();
                return locTangDan;
            }
            else if (loc == 1)
            {
                List<HoaDon> locGiamDan = timkiem.OrderByDescending(p => p.NgayTao).ToList();
                return locGiamDan;
            }
            return timkiem;
        }

        public bool UpdateGhiChuHD(Guid idhd, string ghichu)
        {
            try
            {
                var hd = reposHoaDon.GetAll().FirstOrDefault(c=>c.ID == idhd);
                if(ghichu == "null")
                {
                    hd.GhiChu = null;
                }
                else
                {
                    hd.GhiChu = ghichu;
                }
                reposHoaDon.Update(hd);
                return true;

            }catch(Exception ex)
            {
                return false;
            }
        }

        public bool UpdateHoaDon(HoaDonThanhToanRequest hoaDon)
        {
            var update = reposHoaDon.GetAll().FirstOrDefault(p => p.ID == hoaDon.Id);
            //ChiTietPTTT ctPTTT = new ChiTietPTTT()
            //{
            //    ID = new Guid(),
            //    SoTien = hoaDon.TongTien,
            //    TrangThai = 0,
            //    IDHoaDon = update.ID,
            //    IDPTTT = hoaDon.IdPTTT,
            //};
            //reposChiTietPTTT.Add(ctPTTT);

            //Lưu tiền vào HDCT
            var lsthdct = context.ChiTietHoaDons.Where(c => c.IDHoaDon == hoaDon.Id).ToList();
            //Xóa lsthdct có số lượng = 0
            var delete = lsthdct.Where(c => c.SoLuong == 0).ToList();
            context.ChiTietHoaDons.RemoveRange(delete);
            context.SaveChanges();
            lsthdct = context.ChiTietHoaDons.Where(c => c.IDHoaDon == hoaDon.Id).ToList();
            foreach (var item in lsthdct)
            {
                var GiaBan = from ctsp in context.ChiTietSanPhams
                             join km in context.KhuyenMais
                             on ctsp.IDKhuyenMai equals km.ID into joinResult
                             from km in joinResult.DefaultIfEmpty()
                             where ctsp.ID == item.IDCTSP
                             select ctsp.IDKhuyenMai == null ? ctsp.GiaBan : (km.TrangThai == 0 ? (ctsp.GiaBan * (100 - km.GiaTri) / 100) : (ctsp.GiaBan - km.GiaTri));
                int DonGia = GiaBan.FirstOrDefault();
                item.DonGia = DonGia;

                context.ChiTietHoaDons.Update(item);
                context.SaveChanges();
            }
            //Xóa đánh giá
            var deletedg = context.DanhGias.Where(c => lsthdct.Select(x => x.ID).Contains(c.ID)).ToList();
            context.DanhGias.RemoveRange(deletedg);
            context.SaveChanges();
            //Update LSTD tích
            var lstd = reposLichSuTichDiem.GetAll().FirstOrDefault(c => c.IDHoaDon == hoaDon.Id);
            if (lstd != null)
            {
                lstd.Diem = hoaDon.DiemTichHD;
                reposLichSuTichDiem.Update(lstd);
                // Tạo ls tiêu điểm
                if (hoaDon.DiemSD > 0)
                {
                    LichSuTichDiem lstieudiem = new LichSuTichDiem()
                    {
                        ID = new Guid(),
                        IDHoaDon = lstd.IDHoaDon,
                        IDKhachHang = lstd.IDKhachHang,
                        Diem = hoaDon.DiemSD,
                        TrangThai = 0,
                        IDQuyDoiDiem = lstd.IDQuyDoiDiem,
                    };
                    reposLichSuTichDiem.Add(lstieudiem);
                }
                // Thêm điểm cho Khách hàng và trừ
                var kh = reposKhachHang.GetAll().FirstOrDefault(c => c.IDKhachHang == lstd.IDKhachHang);
                kh.DiemTich += hoaDon.DiemTichHD;
                kh.DiemTich -= hoaDon.DiemSD;
                reposKhachHang.Update(kh);
            }
            // UpdateHD
            update.IDNhanVien = hoaDon.IdNhanVien;
            update.NgayThanhToan = hoaDon.NgayThanhToan;
            update.TrangThaiGiaoHang = hoaDon.TrangThai;
            update.TongTien = hoaDon.TongTien;
            update.ThueVAT = hoaDon.ThueVAT;
            update.PhuongThucThanhToan = hoaDon.PTTT;
            return reposHoaDon.Update(update);
        }

        //public bool UpdatePTTT(PhuongThucThanhToan pttt)
        //{
        //    try
        //    {
        //        var phuongttt = reposPTTT.GetAll().FirstOrDefault(p => p.ID == pttt.ID);
        //        pttt.Ten = phuongttt.Ten;
        //        pttt.TrangThai = phuongttt.TrangThai;
        //        reposPTTT.Update(pttt);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public bool UpdateTrangThaiGiaoHang(Guid idHoaDon, int trangThai, Guid idNhanVien)
        {
            var update = reposHoaDon.GetAll().FirstOrDefault(p => p.ID == idHoaDon);
            List<ChiTietHoaDon> chitiethoadon = reposChiTietHoaDon.GetAll().Where(p => p.IDHoaDon == idHoaDon).ToList();
            if (update != null)
            {
                if (trangThai == 5)
                {
                    foreach (var item in chitiethoadon)
                    {
                        var CTsanPham = repsCTSanPham.GetAll().FirstOrDefault(p => p.ID == item.IDCTSP);
                        CTsanPham.SoLuong += item.SoLuong;
                        repsCTSanPham.Update(CTsanPham);
                    }
                }
                if (trangThai == 6)
                {
                    update.NgayThanhToan = DateTime.Now;
                }
                update.TrangThaiGiaoHang = trangThai;
                update.IDNhanVien = idNhanVien;
                reposHoaDon.Update(update);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
