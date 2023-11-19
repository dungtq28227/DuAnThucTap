namespace AppData.Models
{
    public class QuyDoiDiem
    {
        public Guid ID { get; set; }
        public int SoDiem { get; set; }
        public int TiLeTichDiem { get; set; }
        public int TiLeTieuDiem { get; set; }
        public int TrangThai { get; set; }
        public virtual IEnumerable<LichSuTichDiem> LichSuTichDiems { get; set; }
    }
}
