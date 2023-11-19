using AppAPI.IServices;
using AppAPI.Services;
using AppData.IRepositories;
using AppData.Models;
using AppData.Repositories;
using AppData.ViewModels;
using AppData.ViewModels.SanPham;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangController : ControllerBase
    {
        private readonly IKhachHangService _khachHangService;
        private readonly AssignmentDBContext _dbcontext;
        public KhachHangController()
        {
            _khachHangService = new KhachHangService();
            _dbcontext = new AssignmentDBContext();
            
        }
        // GET: api/<SanPhamController>
        [HttpGet]
        public List<KhachHang> GetAllKhachHang()
        {
            return _khachHangService.GetAll();
        }
        [Route("TimKiemKH")]
        [HttpGet]
        public List<KhachHang> GetAllKhachHang(string? Ten, string? SDT)
        {
            return _dbcontext.KhachHangs.Where(x=>x.SDT.Contains(SDT)|| x.Ten.Contains(Ten)|| x.SDT.Contains(SDT) || x.Ten.Contains(Ten)).ToList();
        }
        [Route("GetById")]
        [HttpGet]
        public KhachHang GetById(Guid id)
        {
            return _khachHangService.GetById(id);
        }
        [HttpGet("GetKhachHangByEmail")] 
        public KhachHangViewModel GetKhachHangByEmail(string email)
        {
            var temp = _dbcontext.KhachHangs.FirstOrDefault(x => x.Email == email);
            if (temp != null)
            {
                var khachHang = new KhachHangViewModel()
                {
                    Id = temp.IDKhachHang,
                    Email = temp.Email
                };
                return khachHang;
            }
            else return new KhachHangViewModel();
        }
        [HttpPost("ChangeForgotPassword")]
        public async Task<bool> ChangeForgotPassword(KhachHangViewModel khachHang)
        {
            try
            {
                var temp = _dbcontext.KhachHangs.First(x => x.IDKhachHang == khachHang.Id);
                temp.Password = khachHang.Password;
                _dbcontext.Update(temp);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //Nhinh
        [HttpGet("getBySDT")]
        public KhachHang GetBySDT(string sdt)
        {
            return _khachHangService.GetBySDT(sdt);
        }

        // GET api/<SanPhamController>/5
        [Route("PostKHView")]
        [HttpPost]
        public bool PostKHView(KhachHangView khv)
        {
            khv.IDKhachHang = Guid.NewGuid();
            KhachHang kh = new KhachHang();
            kh.IDKhachHang = khv.IDKhachHang;
            kh.Ten = khv.Ten;
            kh.Password = khv.Password;
            kh.GioiTinh=khv.GioiTinh;
            kh.NgaySinh=khv.NgaySinh;
            kh.Email = khv.Email;
            kh.DiaChi=khv.DiaChi;
            kh.SDT = khv.SDT;
            kh.TrangThai=1;
            kh.DiemTich = 0;
            _dbcontext.KhachHangs.Add(kh);
            GioHang gh= new GioHang();
            gh.IDKhachHang=kh.IDKhachHang;
            gh.NgayTao=DateTime.Now;
            _dbcontext.GioHangs.Add(gh);
            _dbcontext.SaveChanges();
             
            return true;
        }
       


        // POST api/<SanPhamController>
        [HttpPost]
        public async Task<IActionResult> Post(KhachHangViewModel khachHang)
        {
            var kh = await _khachHangService.Add(khachHang);
            if (kh == null)
            {
                return BadRequest();
            }

            return Ok("Đăng ký thành công");
        }
        // PUT api/<SanPhamController>/5
        [Route("{id}")]
        [HttpPut]
        public bool PutKhView(KhachHangView khv)
        {
            var kh = _khachHangService.GetById(khv.IDKhachHang);
            if (kh != null)
            {
                
                kh.Ten = khv.Ten;
                kh.Password = khv.Password;
                kh.GioiTinh = khv.GioiTinh;
                kh.NgaySinh = khv.NgaySinh;
                kh.Email = khv.Email;
                kh.DiaChi = khv.DiaChi;
                kh.SDT = khv.SDT;
                kh.TrangThai = khv.TrangThai;
                
                _dbcontext.KhachHangs.Update(kh);
                _dbcontext.SaveChanges();
                return true;
            }
            return false;

        }

        // DELETE api/<SanPhamController>/5
        [HttpDelete("{id}")]
        public bool Delete(Guid id)
        {
            var result = _khachHangService.Delete(id);
            return result;
        }
    }
}
