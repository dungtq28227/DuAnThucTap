using AppData.Models;
using AppData.ViewModels;
using AppView.PhanTrang;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Net.Http;
using System.Text;

namespace AppView.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly AssignmentDBContext dBContext;
        public NhanVienController()
        {
            _httpClient = new HttpClient();
            dBContext = new AssignmentDBContext();
        }
        public int PageSize = 10;

        public async Task<IActionResult> Show(int ProductPage= 1)
        {
            string apiUrl = $"https://localhost:7095/api/NhanVien/GetAll";
            var response = await _httpClient.GetAsync(apiUrl);
            string apiData = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<NhanVien>>(apiData);
            return View(new PhanTrangNhanVien
            {
                listNv = users
                        .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = users.Count()
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> SearchTheoTen(string? Ten,int ProductPage = 1)
        {
            string apiUrl = $"https://localhost:7095/api/NhanVien/TimKiemNhanVien?Ten={Ten}";
            var response = await _httpClient.GetAsync(apiUrl);
            string apiData = await response.Content.ReadAsStringAsync();
            var users= JsonConvert.DeserializeObject<List<NhanVien>>(apiData);

            return View("Show", new PhanTrangNhanVien
            {
                listNv = users
                         .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = users.Count()
                }
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                string apiUrl = $"https://localhost:7095/api/NhanVien/DangKyNhanVien?ten={nhanVien.Ten}&email={nhanVien.Email}&sdt={nhanVien.SDT}&diachi={nhanVien.DiaChi}&idVaiTro={nhanVien.IDVaiTro}&trangthai={nhanVien.TrangThai}&password={nhanVien.PassWord}";
                var reponsen = await _httpClient.PostAsync(apiUrl, null);
                if (reponsen.IsSuccessStatusCode)
                {
                    return RedirectToAction("Show");
                }
            }
            return View(nhanVien);

        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            string apiUrl = $"https://localhost:7095/api/NhanVien/GetById?id={id}";
            var response = await _httpClient.GetAsync(apiUrl);
            string apiData = await response.Content.ReadAsStringAsync();

            var user = JsonConvert.DeserializeObject<NhanVien>(apiData);
            var vt = await dBContext.VaiTros.FindAsync(user.IDVaiTro);
            ViewBag.TenVaiTro = vt.Ten;
            return View(user);
        }

        public IActionResult Edit(Guid id)
        {
            string apiUrl = $"https://localhost:7095/api/NhanVien/GetById?id={id}";
            var response = _httpClient.GetAsync(apiUrl).Result;
            var apiData = response.Content.ReadAsStringAsync().Result;
            var user = JsonConvert.DeserializeObject<NhanVien>(apiData);
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, NhanVien nv)
        {
            if (ModelState.IsValid)
            {
                string apiUrl = $"https://localhost:7095/api/NhanVien/{id}?ten={nv.Ten}&email={nv.Email}&password={nv.PassWord}&sdt={nv.SDT}&diachi={nv.DiaChi}&trangthai={nv.TrangThai}&idvaitro={nv.IDVaiTro}";

                var reponsen = await _httpClient.PutAsync(apiUrl, null);
                if (reponsen.IsSuccessStatusCode)
                {
                    return RedirectToAction("Show");
                }
            }
            return View(nv);

        }
        public async Task<IActionResult> Delete(Guid id)
        {
            string apiUrl = $"https://localhost:7095/api/NhanVien/{id}";
            var reposen = await _httpClient.DeleteAsync(apiUrl);
            if (reposen.IsSuccessStatusCode)
            {
                return RedirectToAction("Show");
            }
            return RedirectToAction("Show");
            //else
            //{
            //    // Log the error to the console.
            //    Console.WriteLine(reposen.StatusCode);
            //    Console.WriteLine(reposen.ReasonPhrase);
            //}

            //return View("DeleteError");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Next(int ProductPage = 1)
        {
            // Chuyển sang trang tiếp theo
            ProductPage++;

            // Gọi phương thức Show() để hiển thị trang tiếp theo
            return await Show(ProductPage);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Previous(int ProductPage = 1)
        {
            // Giảm trang hiện tại
            ProductPage--;

            // Gọi phương thức Show() để hiển thị trang trước đó
            return await Show(ProductPage);
        }

    }
}
