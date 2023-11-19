using AppAPI.IServices;
using AppAPI.Services;
using AppData.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GioHangController : ControllerBase
    {
        private readonly IGioHangServices gioHangServices;
        public GioHangController(GioHangServices gioHangServices)
        {
            this.gioHangServices = gioHangServices;
        }
        // GET: api/<GioHangController>
        [HttpGet]
        public List<GioHang> Get()
        {
            return gioHangServices.GetAll();
        }

        // GET api/<GioHangController>/5
        [HttpGet("{id}")]
        public GioHang Get(Guid id)
        {
            return gioHangServices.GetById(id);
        }


        // POST api/<GioHangController>
        [HttpPost]
        public bool Post(Guid IdKhachHang, DateTime ngaytao)
        {
            return gioHangServices.Add(IdKhachHang, ngaytao);
        }

        // PUT api/<GioHangController>/5
        [HttpPut("{id}")]
        public bool Put(Guid id, DateTime ngaytao)
        {
            var gioHang = gioHangServices.GetById(id);
            if (gioHang != null)
            {
                return gioHangServices.Update(gioHang.IDKhachHang, ngaytao);
            }
            else
            {
                return false;
            }
        }

        // DELETE api/<GioHangController>/5
        [HttpDelete("{id}")]
        public bool Delete(Guid id)
        {
            var gioHang = gioHangServices.GetById(id);
            if (gioHang != null)
            {
                return gioHangServices.Delete(gioHang.IDKhachHang);
            }
            else
            {
                return false;
            }
        }
    }
}
