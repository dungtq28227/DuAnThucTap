namespace AppData.Models
{
    public class LichSuTichDiem
    {
        public Guid ID { get; set; }
        public int Diem { get; set; }
        public int TrangThai { get; set; }
        public Guid? IDKhachHang { get; set; }
        public Guid IDQuyDoiDiem { get; set; }
        public Guid IDHoaDon { get; set; }
        public virtual KhachHang? KhachHang { get; set; }
        public virtual QuyDoiDiem? QuyDoiDiem { get; set; }
        public virtual HoaDon? HoaDon { get; set; }
    }
}
