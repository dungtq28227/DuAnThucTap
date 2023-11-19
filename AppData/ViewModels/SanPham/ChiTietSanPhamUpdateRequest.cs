using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.ViewModels.SanPham
{
    public class ChiTietSanPhamUpdateRequest
    {
        public Guid IDSanPham { get; set; }
        public List<ChiTietSanPhamRequest> ChiTietSanPhams { get; set; }
        public string TrangThai {  get; set; }
    }
}
