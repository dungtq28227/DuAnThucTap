using AppData.Models;
using AppData.ViewModels.SanPham;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using AppView.PhanTrang;

namespace AppView.Controllers
{
    public class LoaiSPController : Controller
    {
        private readonly HttpClient _httpClient;
        public LoaiSPController()
        {
            _httpClient = new HttpClient();
        }
        public int PageSize = 8;
        // laam them 
        public async Task<IActionResult> Show(int ProductPage = 1)
        {
            string apiUrl = "https://localhost:7095/api/LoaiSP/getAll";
            var response = await _httpClient.GetAsync(apiUrl);
            string apiData = await response.Content.ReadAsStringAsync();
            var LoaiSPs = JsonConvert.DeserializeObject<List<LoaiSP>>(apiData);
            return View(new PhanTrangLoaiSP
            {
                listlsp = LoaiSPs
                         .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = LoaiSPs.Count()
                }

            }
                 );
        }
        // Tim kiem Loai SP theo ten
        [HttpGet]
        public async Task<IActionResult> TimKiemLoaiSPTheoTen(string Ten,int ProductPage = 1)
        {
            string apiUrl = "https://localhost:7095/api/LoaiSP/getAll";
            var response = await _httpClient.GetAsync(apiUrl);
            string apiData = await response.Content.ReadAsStringAsync();
            var LoaiSPs = JsonConvert.DeserializeObject<List<LoaiSP>>(apiData);
            return View(new PhanTrangLoaiSP
            {
                listlsp = LoaiSPs.Where(x=>x.Ten.Contains(Ten))
                         .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = LoaiSPs.Count()
                }

            }
                 );
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(LoaiSP lsp)
        {
                string apiURL = $"https://localhost:7095/api/LoaiSP/save";
                var content = new StringContent(JsonConvert.SerializeObject(lsp), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(apiURL, content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Show");
                }
                return View();
                    
          
        }
        public async Task<IActionResult> Details(Guid id)
        {

            string apiUrl = $"https://localhost:7095/api/LoaiSP/getById/{id}";
            var response = await _httpClient.GetAsync(apiUrl);
            string apiData = await response.Content.ReadAsStringAsync();
            var LoaiSPs = JsonConvert.DeserializeObject<LoaiSP>(apiData);
            return View(LoaiSPs);

        }
        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var url = $"https://localhost:7095/api/LoaiSP/getById/{id}";
            var response = _httpClient.GetAsync(url).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            var LoaiSPs = JsonConvert.DeserializeObject<LoaiSPRequest>(result);
            return View(LoaiSPs);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LoaiSPRequest lsp,Guid id)
        {
            if (lsp == null) return BadRequest();
            string apiURL = $"https://localhost:7095/api/LoaiSP/save";
            var content = new StringContent(JsonConvert.SerializeObject(lsp), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiURL, content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Show");
            }
            else
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi chỉnh sửa.");
                return View(lsp);
            }
            return View();
            
        }
    }
}
