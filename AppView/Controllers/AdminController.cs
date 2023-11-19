using AppData.Models;
using AppData.ViewModels.SanPham;
using AppView.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppView.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IFileService _iFileService;
        public AdminController(IWebHostEnvironment hostEnvironment,IFileService iFileService)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7095/api/");
            _hostEnvironment = hostEnvironment;
            _iFileService = iFileService;
        }
        public IActionResult HomePageAdmin(Guid id)
        {
            return RedirectToAction("BanHang", "BanHangTaiQuay");
        }
        public IActionResult ProductManager()
        {
            var response = _httpClient.GetAsync(_httpClient.BaseAddress+ "SanPham/getAll").Result;
            var lstSanPham = JsonConvert.DeserializeObject<List<SanPhamViewModel>>(response.Content.ReadAsStringAsync().Result);
            return View(lstSanPham);
        }
        public JsonResult GetLoaiSPCha()
        {
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "SanPham/GetAllLoaiSPCha").Result;
            var loaiSP = JsonConvert.DeserializeObject<List<LoaiSP>>(response.Content.ReadAsStringAsync().Result);
            return Json(loaiSP);
        }
        public JsonResult GetAllMauSac()
        {
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "SanPham/GetAllMauSac").Result;
            var mauSac = JsonConvert.DeserializeObject<List<MauSac>>(response.Content.ReadAsStringAsync().Result);
            return Json(mauSac);
        }
        public JsonResult GetAllKichCo()
        {
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "SanPham/GetAllKichCo").Result;
            var kichCo = JsonConvert.DeserializeObject<List<KichCo>>(response.Content.ReadAsStringAsync().Result);
            return Json(kichCo);
        }
        public JsonResult GetAllChatLieu()
        {
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "SanPham/GetAllChatLieu").Result;
            var chatLieu = JsonConvert.DeserializeObject<List<ChatLieu>>(response.Content.ReadAsStringAsync().Result);
            return Json(chatLieu);
        }
        public JsonResult GetLoaiSPCon(string tenLoaiSPCha)
        {
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "SanPham/GetAllLoaiSPCon?tenLoaiSPCha=" + tenLoaiSPCha).Result;
            var loaiSP = JsonConvert.DeserializeObject<List<LoaiSP>>(response.Content.ReadAsStringAsync().Result);
            return Json(loaiSP);
        }
        [HttpGet]
        public IActionResult AddSanPham()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddSanPham(SanPhamRequest sanPhamRequest)
        {
            HttpResponseMessage response = _httpClient.PostAsJsonAsync(_httpClient.BaseAddress + "SanPham/AddSanPham", sanPhamRequest).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["UpdateChiTietSanPham"] = response.Content.ReadAsStringAsync().Result;
                TempData["MauSac"] = JsonConvert.SerializeObject(sanPhamRequest.MauSacs);
                return RedirectToAction("UpdateChiTietSanPham");
            }
            else return BadRequest();
        }
        [HttpGet]
        public IActionResult ProductDetail(string idSanPham)
        {
            var response = _httpClient.GetAsync(_httpClient.BaseAddress+ "SanPham/GetAllChiTietSanPhamAdmin?idSanPham=" + idSanPham).Result;
            var lstSanPham = JsonConvert.DeserializeObject<List<ChiTietSanPhamViewModelAdmin>>(response.Content.ReadAsStringAsync().Result);
            TempData["IDSanPham"] = idSanPham;
            return View(lstSanPham);
        }
        [HttpGet]
        public IActionResult AddChiTietSanPham(string idSanPham)
        {
            TempData["IDSanPham"] = idSanPham;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddChiTietSanPham(ChiTietSanPhamUpdateRequest request)
        {
            string idSanPham = TempData.Peek("IDSanPham").ToString();
            request.IDSanPham = new Guid(idSanPham);
            var response = await _httpClient.PostAsJsonAsync(_httpClient.BaseAddress + "SanPham/AddChiTietSanPham", request);
            if (response.IsSuccessStatusCode)
            {
                List<AnhRequest> lstAnhRequest = JsonConvert.DeserializeObject<List<AnhRequest>>(response.Content.ReadAsStringAsync().Result);
                TempData["MauSacs"] = JsonConvert.SerializeObject(lstAnhRequest);
                return RedirectToAction("AddAnhToMauSac");
            }
            else return BadRequest();
        }
        [HttpGet]
        public IActionResult AddAnhToMauSac()
        {
            var lstAnhRequest = JsonConvert.DeserializeObject<List<AnhRequest>>(TempData["MauSacs"].ToString());
            return View(lstAnhRequest);
        }
        //[HttpPost]
        //public IActionResult AddAnhToMauSac(List<string> idMauSacs,List<IFormFile> images)
        //{
        //    string wwwrootPath = _hostEnvironment.WebRootPath;
        //    string idSanPham = TempData["IDSanPham"].ToString();
        //    List<AnhRequest> lstAnhRequest = new List<AnhRequest>();
        //    for (var i = 0; i < images.Count; i++)
        //    {
        //        lstAnhRequest.Add(new AnhRequest() { IDSanPham = new Guid(idSanPham), IDMauSac = new Guid(idMauSacs[i]), DuongDan = _iFileService.AddFile(images[i], wwwrootPath).Result });
        //    }
        //    HttpResponseMessage response = _httpClient.PostAsJsonAsync(_httpClient.BaseAddress + "SanPham/AddAnh", lstAnhRequest).Result;
        //    if (response.IsSuccessStatusCode) return RedirectToAction("ProductManager");
        //    else return BadRequest();
        //}
        [HttpGet]
        public IActionResult UpdateChiTietSanPham()
        {
            var request = JsonConvert.DeserializeObject<ChiTietSanPhamUpdateRequest>(TempData["UpdateChiTietSanPham"].ToString());
            TempData["SanPham"]=request.IDSanPham.ToString();
            return View(request);
        }
        [HttpPost]
        public IActionResult UpdateChiTietSanPham(ChiTietSanPhamUpdateRequest request)
        {
            request.IDSanPham = new Guid(TempData.Peek("SanPham").ToString());
            var response = _httpClient.PostAsJsonAsync(_httpClient.BaseAddress+ "SanPham/AddChiTietSanPham",request).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("AddAnhToSanPham");
            }
            else return BadRequest();
        }
        [HttpGet]
        public IActionResult AddAnhToSanPham()
        {
            var lst = JsonConvert.DeserializeObject<List<MauSac>>(TempData["MauSac"].ToString());
            return View(lst);
        }
        [HttpPost]
        public IActionResult AddAnhToSanPham(List<string> maMaus, List<IFormFile> images)
        {
            string wwwrootPath = _hostEnvironment.WebRootPath;
            string idSanPham = TempData["SanPham"].ToString();
            List<AnhRequest> lstAnhRequest = new List<AnhRequest>();
            for (var i = 0; i < images.Count; i++)
            {
                lstAnhRequest.Add(new AnhRequest() { IDSanPham = new Guid(idSanPham), MaMau = maMaus[i], DuongDan = _iFileService.AddFile(images[i], wwwrootPath).Result });
            }
            HttpResponseMessage response = _httpClient.PostAsJsonAsync(_httpClient.BaseAddress + "SanPham/AddAnh", lstAnhRequest).Result;
            if (response.IsSuccessStatusCode) return RedirectToAction("ProductManager");
            else return BadRequest();
        }
    }
}
