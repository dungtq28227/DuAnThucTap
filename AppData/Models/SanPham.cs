using System.ComponentModel.DataAnnotations;

namespace AppData.Models
{
    public class SanPham
    {
        public Guid ID { get; set; }
        [StringLength(40, ErrorMessage = "Ten san pham khong duoc dai qua 40 tu.")]
        public string Ten { get; set; }
        [Required]
        public string MoTa { get; set; }

        public int TrangThai { get; set; }
        public int TongSoSao { get; set; }
        public int TongDanhGia { get; set; }
        public Guid IDLoaiSP { get; set; }
        public Guid IDChatLieu { get; set; }
        public virtual LoaiSP? LoaiSP { get; set; }
        public virtual ChatLieu ChatLieu { get; set; }
        public virtual IEnumerable<ChiTietSanPham> ChiTietSanPhams { get; set; }
        public virtual IEnumerable<Anh> Anhs { get; set; }
    }
}
