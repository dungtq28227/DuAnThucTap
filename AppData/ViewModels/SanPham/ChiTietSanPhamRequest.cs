using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.ViewModels.SanPham
{
    public class ChiTietSanPhamRequest
    {
        public Guid IDChiTietSanPham { get; set; }
        public Guid IDMauSac { get; set; }
        public Guid IDKichCo { get; set; }
        public string? TenKichCo { get; set; }
        public string? TenMauSac { get; set; }
        public string? MaMau { get; set; }
        public int SoLuong { get; set; }
        public int GiaBan { get; set; }
    }
}
