using AppAPI.IServices;
using AppData.Models;
using AppData.ViewModels.SanPham;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Services
{
    public class LoaiSPService : ILoaiSPService
    {
        private readonly AssignmentDBContext _context;
        public LoaiSPService()
        {
            _context = new AssignmentDBContext();
        }
        #region LoaiSP
        public async Task<bool> DeleteLoaiSP(Guid id)
        {
            var lsp = await _context.LoaiSPs.FindAsync(id);
            if (lsp == null) throw new Exception($"Không tìm thấy Loại sản phẩm: {id}");
            // Check LoaiSP đag đc sử dụng k
            if(_context.SanPhams.Any(c=>c.IDLoaiSP == id)) return false;
            _context.LoaiSPs.Remove(lsp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<LoaiSP>> GetAllLoaiSP()
        {
            return await _context.LoaiSPs.AsNoTracking().ToListAsync();
        }

        public async Task<LoaiSP> GetLoaiSPById(Guid id)
        {
            return await _context.LoaiSPs.FindAsync(id);
        }

        public async Task<LoaiSP> SaveLoaiSP(LoaiSPRequest lsp)
        {
            var Lsp = await _context.LoaiSPs.FindAsync(lsp.ID);
            //Check tồn tại
            if (Lsp != null) //Update
            {
                Lsp.Ten = lsp.Ten;
                Lsp.TrangThai = lsp.TrangThai;
                Lsp.IDLoaiSPCha = lsp.IDLoaiSPCha;
                _context.LoaiSPs.Update(Lsp);
                await _context.SaveChangesAsync();
                return Lsp;
            }
            else // Tạo mới
            {
                LoaiSP loaiSP = new LoaiSP()
                {
                    ID = new Guid(),
                    Ten = lsp.Ten,
                    IDLoaiSPCha = lsp.IDLoaiSPCha,
                    TrangThai = lsp.TrangThai,
                };
                await _context.AddAsync(loaiSP);
                await _context.SaveChangesAsync();
                return loaiSP;
            }
        }

        public bool CheckTrungLoaiSP(LoaiSPRequest lsp)
        {
            if (_context.LoaiSPs.Any(c => c.Ten == lsp.Ten && c.ID != lsp.ID))
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
