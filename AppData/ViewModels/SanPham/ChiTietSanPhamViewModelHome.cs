using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.ViewModels.SanPham
{
    public class ChiTietSanPhamViewModelHome
    {
        public string Ten { get; set; }
        public float SoSao { get; set; }
        public int SoDanhGia { get; set; }
        public List<MauSac> MauSacs { get; set; }
        public List<Anh> Anhs { get; set; }
        public List<KichCo> KichCos { get; set; }
        public List<ChiTietSanPham> ChiTietSanPhams { get; set; }
        public string MoTa { get; set; }
        public List<DanhGiaViewModel> LSTDanhGia { get; set;}

        public List<SanPhamTuongTuViewModel> LSTSPTuongTu { get; set; }
    }
}
