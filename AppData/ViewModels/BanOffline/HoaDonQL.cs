using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.ViewModels.BanOffline
{
    public class HoaDonQL
    {
        public Guid Id { get; set; }
        public string MaHD { get; set; }
        public DateTime? ThoiGian { get; set; }
        public string? KhachHang { get; set; }
        public int? KhachTra { get; set; }
        public int TrangThai { get; set; }
        public int LoaiHD { get; set; }
    }
}
