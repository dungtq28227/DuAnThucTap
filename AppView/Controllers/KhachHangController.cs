using AppData.Models;
using AppData.ViewModels;
using AppView.PhanTrang;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace AppView.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly HttpClient httpClients;

        public KhachHangController()
        {
            httpClients = new HttpClient();
        }

        public int PageSize = 8;
        // Get ALl KH
        public async Task<IActionResult> GetAllKhachHang(int ProductPage = 1)
        {
            string apiUrl = "https://localhost:7095/api/KhachHang";
            var response = await httpClients.GetAsync(apiUrl);
            string apiData = await response.Content.ReadAsStringAsync();
            var kh = JsonConvert.DeserializeObject<List<KhachHangView>>(apiData);
            return View(new PhanTrangKhachHang
            {
                listkh = kh
                       .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = kh.Count()
                }

            });
        }
        // tim kiem KH theo Ten Hoac SDT
        [HttpGet]
        public async Task<IActionResult> GetAllKHTheoTimKiem(string? Ten, string? SDT,int ProductPage = 1)
        {
            string apiUrl = $"https://localhost:7095/api/KhachHang/TimKiemKH?Ten={Ten}&SDT={SDT}";
            var response = await httpClients.GetAsync(apiUrl);
            string apiData = await response.Content.ReadAsStringAsync();
            var kh = JsonConvert.DeserializeObject<List<KhachHangView>>(apiData);
            return View("GetAllKhachHang", new PhanTrangKhachHang
            {
                listkh = kh
                       .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = kh.Count()
                }

            });
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Create(KhachHangView kh)
        {
            var url = $"https://localhost:7095/api/KhachHang/PostKHView";
            var response = await httpClients.PostAsJsonAsync(url, kh);
            if (response.IsSuccessStatusCode) return RedirectToAction("GetAllKhachHang");
            return View();
        }
        [HttpGet]

        public async Task<IActionResult> Details(Guid id)
        {

            string apiUrl = "https://localhost:7095/api/KhachHang/GetById?id=" + id;
            var response = await httpClients.GetAsync(apiUrl);
            string apiData = await response.Content.ReadAsStringAsync();
            var NhaCungCaps = JsonConvert.DeserializeObject<KhachHangView>(apiData);
            return View(NhaCungCaps);

        }
        [HttpGet]
        public async Task<IActionResult> Updates(Guid id)
        {
            string apiUrl = $"https://localhost:7095/api/KhachHang/GetById?id={id}";
            var response = await httpClients.GetAsync(apiUrl);
            string apiData = await response.Content.ReadAsStringAsync();
            var kh = JsonConvert.DeserializeObject<KhachHangView>(apiData);

            return View(kh);
        }
        [HttpPost]

        public async Task<IActionResult> Update(KhachHangView kh)
        {

            var url =
                $"https://localhost:7095/api/KhachHang/{kh.IDKhachHang}";
            var response = await httpClients.PutAsJsonAsync(url, kh);
            if (response.IsSuccessStatusCode) return RedirectToAction("GetAllKhachHang");


            return View();
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            var url = $"https://localhost:7095/api/KhachHang/{id}";
            var response = await httpClients.DeleteAsync(url);
            if (response.IsSuccessStatusCode) return RedirectToAction("GetAllKhachHang");
            return BadRequest();
        }
    }
}
