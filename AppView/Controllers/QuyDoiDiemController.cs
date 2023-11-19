using AppData.Models;
using AppData.ViewModels;
using AppView.PhanTrang;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppView.Controllers
{
    public class QuyDoiDiemController : Controller
    {
        private readonly HttpClient _httpClient;
        public QuyDoiDiemController()
        {
            _httpClient = new HttpClient();
        }
        public int PageSize = 8;
        [HttpGet]
        public async Task<IActionResult> GetAllQuyDoiDiem(int ProductPage = 1)
        {
            string apiURL = $"https://localhost:7095/api/QuyDoiDiem";
            var response = await _httpClient.GetAsync(apiURL);
            var apiData = await response.Content.ReadAsStringAsync();
            var roles = JsonConvert.DeserializeObject<List<QuyDoiDiem>>(apiData);
            return View(new PhanTrangQuyDoiDiem
            {
                listqdd= roles
                        .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = roles.Count()
                }

            }
                );

        }
        // tim kiem ten
       
        // create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(QuyDoiDiem qdd)
        {


            var response = await _httpClient.PostAsync($"https://localhost:7095/api/QuyDoiDiem?sodiem={qdd.SoDiem}&TiLeTichDiem={qdd.TiLeTichDiem}&TiLeTieuDiem={qdd.TiLeTieuDiem}&TrangThai={qdd.TrangThai}", null);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetAllQuyDoiDiem");
            }
            return View();
        }
        // update
        [HttpGet]
        public IActionResult Updates(Guid id)
        {

            var url = $"https://localhost:7095/api/QuyDoiDiem/{id}";
            var response = _httpClient.GetAsync(url).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            var KhuyenMais = JsonConvert.DeserializeObject<QuyDoiDiem>(result);
            return View(KhuyenMais);
        }

        [HttpPost]

        public async Task<IActionResult> Updates(QuyDoiDiem qdd)
        {


            var response = await _httpClient.PutAsync($"https://localhost:7095/api/QuyDoiDiem/{qdd.ID}?sodiem={qdd.SoDiem}&TiLeTichDiem={qdd.TiLeTichDiem}&TiLeTieuDiem={qdd.TiLeTieuDiem}&TrangThai={qdd.TrangThai}", null);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetAllQuyDoiDiem");
            }
            return View();
        }
        // delete
        public async Task<IActionResult> Delete(Guid id)
        {
            string apiURL = $"https://localhost:7095/api/QuyDoiDiem/{id}";

            var response = await _httpClient.DeleteAsync(apiURL);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetAllQuyDoiDiem");
            }
            return View();
        }
    }
}
