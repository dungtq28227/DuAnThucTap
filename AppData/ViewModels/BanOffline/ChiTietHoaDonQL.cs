using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.ViewModels.BanOffline
{
    public class ChiTietHoaDonQL
    {
        public Guid Id { get; set; }
        public string MaHD { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public string NhanVien { get; set; }
        public string KhachHang { get; set; }
        public string TrangThai { get; set; }
        public int TichDiemSD { get; set; }
        public int TichDIemHD { get; set; }
        public int? ThueVAT { get; set; }
        public int? TienKhachTra { get; set; }
        public List<HoaDonChiTietViewModel> listsp { get; set; }
        public Voucher voucher { get; set; }
    }
}
