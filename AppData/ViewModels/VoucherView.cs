using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.ViewModels
{
    public  class VoucherView
    {
        public Guid Id { get; set; }
        public string Ten { get; set; }
        public int HinhThucGiamGia { get; set; }//0 là giảm theo %, 1 là giảm thẳng giá tiền
        public int SoTienCan { get; set; }
        public int GiaTri { get; set; }
        public DateTime NgayApDung { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int SoLuong { get; set; }
        public string MoTa { get; set; }
        public int TrangThai { get; set; }
    }
}
