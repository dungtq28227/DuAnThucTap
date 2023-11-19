using AppData.Models;

namespace AppAPI.IServices
{
    public interface IQuyDoiDiemServices
    {
        public bool Add(int sodiem,int TiLeTichDiem,int TiLeTieuDiem,int TrangThai);
        public bool Update(Guid Id, int sodiem, int TiLeTichDiem, int TiLeTieuDiem, int TrangThai);
        public bool Delete(Guid Id);
        public QuyDoiDiem GetById(Guid Id);
        public List<QuyDoiDiem> GetAll();
    }
}
