using AppAPI.IServices;
using AppAPI.Services;
using AppData.ViewModels.SanPham;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiSPController : ControllerBase
    {
        private readonly ILoaiSPService _loaiSPService;
        public LoaiSPController()
        {
            _loaiSPService = new LoaiSPService();
        }
        #region LoaiSP
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var listLsp = await _loaiSPService.GetAllLoaiSP();
            return Ok(listLsp);
        }
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var lsp = await _loaiSPService.GetLoaiSPById(id);
            if (lsp == null) return NotFound();
            return Ok(lsp);
        }
        [HttpPost("save")]
        public async Task<IActionResult> SaveLoaiSP(LoaiSPRequest lsp)
        {
            if (lsp == null) return BadRequest();
            var loaiSP = await _loaiSPService.SaveLoaiSP(lsp);
            return Ok(loaiSP);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteLoaiSP(Guid id)
        {
            var loaiSP = await _loaiSPService.DeleteLoaiSP(id);
            return Ok();
        }
        #endregion
    }
}
