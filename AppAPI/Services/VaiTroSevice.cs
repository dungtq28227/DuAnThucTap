using AppAPI.IServices;
using AppData.Models;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Services
{
    public class VaiTroSevice : IVaiTroService
    {
        private readonly AssignmentDBContext dBContext;

        public VaiTroSevice()
        {
            this.dBContext = new AssignmentDBContext();
        }

        public bool CreateVaiTro(string ten, int trangthai)
        {
            var vaitro = new VaiTro();
            vaitro.ID = Guid.NewGuid();
            vaitro.Ten = ten;
            vaitro.TrangThai = trangthai;
            dBContext.VaiTros.Add(vaitro);
            dBContext.SaveChanges();
            return true;

        }

        public bool DeleteVaiTro(Guid id)
        {
            var vt =  dBContext.VaiTros.FirstOrDefault(a=>a.ID == id);
            if (vt != null)
            {
                dBContext.VaiTros.Remove(vt);
                return true;
            }
            return false;
        }

        public List<VaiTro> GetAllVaiTro()
        {
            return  dBContext.VaiTros.ToList();
        }

        public VaiTro GetVaiTroById(Guid id)
        {
            return  dBContext.VaiTros.FirstOrDefault(x => x.ID == id);
        }

        public bool UpdateVaiTro(Guid id, string ten, int trangthai)
        {
            var vaitro =  dBContext.VaiTros.FirstOrDefault(a=>a.ID == id);
            if (vaitro == null)
            {
                return false;
            }
            else
            {
                vaitro.Ten = ten;
                vaitro.TrangThai = trangthai;
                dBContext.VaiTros.Update(vaitro);
                dBContext.SaveChanges();
                return true;
            }
        }

        
    }
}
