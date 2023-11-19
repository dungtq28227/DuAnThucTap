namespace AppData.Models
{
    public class KhuyenMai
    {
        public Guid ID { get; set; }
        public string Ten { get; set; }
        public int GiaTri { get; set; }
        public DateTime NgayApDung { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string MoTa { get; set; }
        public int TrangThai { get; set; }
        public virtual IEnumerable<ChiTietSanPham> ChiTietSanPhams { get; set; }
    }
}
