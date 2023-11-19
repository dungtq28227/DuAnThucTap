namespace AppData.Models
{
    public class HoaDon
    {
        public Guid ID { get; set; }
        public string MaHD { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public string? TenNguoiNhan { get; set; }
        public string? SDT { get; set; }
        public string? Email { get; set; }
        public string? DiaChi { get; set; }
        public int TienShip { get; set; }
        public int? ThueVAT { get; set; }
        public int? TongTien { get; set; }
        public int LoaiHD { get; set; } // Off-1 // On-0
        public string? PhuongThucThanhToan { get; set; }
        public string? GhiChu { get; set; }
        public int TrangThaiGiaoHang { get; set; }
        //Các trạng thái của đơn hàng
        /*
         * 1-đơn nháp
         * 2-Chờ bàn giao 
         * 3-Đã bàn giao-Đang giao
         * 4-Đã bàn giao-Đang hoàn hàng
         * 5-Hoàn hàng thành công
         * 6-Giao hàng thành công
         * 7-Đơn hủy
         * 8-Hàng thất lạc- hư hỏng
         */
        public Guid? IDNhanVien { get; set; }
        public Guid? IDVoucher { get; set; }
        public virtual IEnumerable<LichSuTichDiem>? LichSuTichDiems { get; set; }
        public virtual NhanVien? NhanVien { get; set; }
        public virtual Voucher? Voucher { get; set; }
        public virtual IEnumerable<ChiTietHoaDon>? ChiTietHoaDons { get; set; }
    }
}
