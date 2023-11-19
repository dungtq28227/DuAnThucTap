using AppData.Models;
using AppData.ViewModels;
using AppView.PhanTrang;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace AppView.Controllers
{
    public class QuanLyKMController : Controller
    {
        private readonly HttpClient _httpClient;
        public QuanLyKMController()
        {
            _httpClient = new HttpClient();
        }
        public int PageSize = 8;
        #region QL Khuyen Mai Cho SP
        // Get All QL Khuyen Mai
        [HttpGet]
        public async Task<IActionResult> GetAllQLKhuyenMaiSP(int ProductPage = 1)
        {// list khuyen mai view
            string apiURL = $"https://localhost:7095/api/KhuyenMai";
            var response = await _httpClient.GetAsync(apiURL);
            var apiData = await response.Content.ReadAsStringAsync();
            var roles = JsonConvert.DeserializeObject<List<KhuyenMaiView>>(apiData);
            ViewBag.KhuyenMaiView = roles;       
            // // list AllViewsp
            string apiURL1 = $"https://localhost:7095/api/KhuyenMai/GetAllSP";
            var response1 = await _httpClient.GetAsync(apiURL1);
            var apiData1 = await response1.Content.ReadAsStringAsync();
            var qlsanphams = JsonConvert.DeserializeObject<List<AllViewSp>>(apiData1);
            return View(new PhanTrangAllQLKMSP
            {
                listallsp = qlsanphams
                        .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = qlsanphams.Count()
                }

            });
        }
        [HttpGet]
        // Tim kiem theo Id khuyen mai, id loai sp, id chat lieu
        public async Task<IActionResult> TimKiemSPByKM(Guid? idkhuyenmai,Guid? idloaisp,Guid? idchatlieu, int ProductPage = 1)
        {// list khuyen mai view
            string apiURL = $"https://localhost:7095/api/KhuyenMai";
            var response = await _httpClient.GetAsync(apiURL);
            var apiData = await response.Content.ReadAsStringAsync();
            var roles = JsonConvert.DeserializeObject<List<KhuyenMaiView>>(apiData);
            ViewBag.KhuyenMaiView = roles;
            // // list AllViewsp
            string apiURL1 = $"https://localhost:7095/api/KhuyenMai/GetAllSPByKmLoaiSPChatLieu?idkm={idkhuyenmai}&idLoaiSP={idloaisp}&idChatLieu={idchatlieu}";
            var response1 = await _httpClient.GetAsync(apiURL1);
            var apiData1 = await response1.Content.ReadAsStringAsync();
            var qlsanphams = JsonConvert.DeserializeObject<List<AllViewSp>>(apiData1);
           
            return View("GetAllQLKhuyenMaiSP",new PhanTrangAllQLKMSP
            {
                listallsp = qlsanphams
                        .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = qlsanphams.Count()
                }

            });
        }
        // add km vo list SP theo id SP
        [HttpGet]
        public async Task<IActionResult> AddKHuyenMaiVoSP(Guid id, int ProductPage = 1)
        {           
            // list khuyen mai view
            string apiURL = $"https://localhost:7095/api/KhuyenMai";
            var response = await _httpClient.GetAsync(apiURL);
            var apiData = await response.Content.ReadAsStringAsync();
            var roles = JsonConvert.DeserializeObject<List<KhuyenMaiView>>(apiData);
            ViewBag.KhuyenMaiView = roles;
            // list AllCTSP by SP
            string apiURL1 = $"https://localhost:7095/api/KhuyenMai/GetAllCTSPBySP?idSanPham={id}";
            var response1 = await _httpClient.GetAsync(apiURL1);
            var apiData1 = await response1.Content.ReadAsStringAsync();
            var bienthes = JsonConvert.DeserializeObject<List<AllViewCTSP>>(apiData1);
            return View( new PhanTrangCTSPBySP
            {
               listallctspbysp = bienthes
                        .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = bienthes.Count()
                }
            });
        }
       
       
        [HttpPost]
        public async Task<IActionResult> AddKHuyenMaiVoSP(Guid idKhuyenMai, List<Guid> bienthes)
        {
            var response = await _httpClient.PutAsJsonAsync($"https://localhost:7095/api/KhuyenMai/AddKmVoBT?IdKhuyenMai={idKhuyenMai}", bienthes);
            if (response.IsSuccessStatusCode) return Json(new { success = true, message = "Cập nhật thành công!" });
            return Json(new { success = false });
        }
        // add km vo list SP theo id SP lay IDKhuyenMai Từ Session 
        [HttpGet]
        public async Task<IActionResult> AddCTSPByIdKMLayTuSession(Guid id, int ProductPage = 1)
        {
            // list khuyen mai view
            string apiURL = $"https://localhost:7095/api/KhuyenMai";
            var response = await _httpClient.GetAsync(apiURL);
            var apiData = await response.Content.ReadAsStringAsync();
            var roles = JsonConvert.DeserializeObject<List<KhuyenMaiView>>(apiData);
            ViewBag.KhuyenMaiView = roles;
            // list AllCTSP by SP
            string apiURL1 = $"https://localhost:7095/api/KhuyenMai/GetAllCTSPBySP?idSanPham={id}";
            var response1 = await _httpClient.GetAsync(apiURL1);
            var apiData1 = await response1.Content.ReadAsStringAsync();
            var bienthes = JsonConvert.DeserializeObject<List<AllViewCTSP>>(apiData1);
            return View(new PhanTrangCTSPBySP
            {
                listallctspbysp = bienthes
                        .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = bienthes.Count()
                }
            });
        }


        [HttpPost]
        public async Task<IActionResult> AddCTSPByIdKMLayTuSession(List<Guid> bienthes)
        {
            // lay IdkhuyenMai Tu session
            var idkhuyenmai = Guid.Parse(HttpContext.Session.GetString("IdKhuyenMai"));
            var response = await _httpClient.PutAsJsonAsync($"https://localhost:7095/api/KhuyenMai/AddKmVoBT?IdKhuyenMai={idkhuyenmai}", bienthes);
            if (response.IsSuccessStatusCode) return Json(new { success = true, message = "Cập nhật thành công!" });
            return Json(new { success = false });
        }
        [HttpPost]
        public async Task<IActionResult>XoaKHuyenMaiRaSP( List<Guid> bienthes)
        {
            var response = await _httpClient.PutAsJsonAsync($"https://localhost:7095/api/KhuyenMai/XoaKmRaBT", bienthes);
            if (response.IsSuccessStatusCode) return Json(new { success = true, message = "Xóa Khuyến Mãi ra  thành công!" });
            return Json(new { success = false });
        }
        #endregion
        #region Khuyen Mai
        // Get All KM
        [HttpGet]
        public async Task<IActionResult> GetAllKM(int ProductPage = 1)
        {// list khuyen mai view
            string apiURL = $"https://localhost:7095/api/KhuyenMai";
            var response = await _httpClient.GetAsync(apiURL);
            var apiData = await response.Content.ReadAsStringAsync();
            var roles = JsonConvert.DeserializeObject<List<KhuyenMaiView>>(apiData);
            ViewBag.KhuyenMaiView = roles;



            return View(new PhanTrangKhuyenMai
            {
                listkms = roles
                        .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = roles.Count()
                }

            });
        }
        // Ti Kiem ten KM
        [HttpGet]
        public async Task<IActionResult> TimKiemTenKM(string TenKM, int ProductPage = 1)
        {// list khuyen mai view
            string apiURL = $"https://localhost:7095/api/KhuyenMai";
            var response = await _httpClient.GetAsync(apiURL);
            var apiData = await response.Content.ReadAsStringAsync();
            var roles = JsonConvert.DeserializeObject<List<KhuyenMaiView>>(apiData);
            ViewBag.KhuyenMaiView = roles;



            return View("GetAllKM", new PhanTrangKhuyenMai
            {
                listkms = roles.Where(x => x.Ten.Contains(TenKM))
                        .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = roles.Count()
                }

            });
        }
        // create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
       
        public async Task<IActionResult> Create(KhuyenMaiView kmv)
        {
            var response = await
           _httpClient.PostAsJsonAsync("https://localhost:7095/api/KhuyenMai", kmv);
            if (response.IsSuccessStatusCode) return RedirectToAction("GetAllKM");
            return View();
        }
        // update
        public  IActionResult Update(Guid id)
        {
           
            var url = $"https://localhost:7095/api/KhuyenMai/{id}";
            var response = _httpClient.GetAsync(url).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            var KhuyenMais = JsonConvert.DeserializeObject<KhuyenMaiView>(result);
            return View(KhuyenMais);
        }

        [HttpPost]
        public async Task<IActionResult> Update(KhuyenMaiView kmv)
        {
            

            var response = await _httpClient.PutAsJsonAsync($"https://localhost:7095/api/KhuyenMai/{kmv.ID}", kmv);
            if (response.IsSuccessStatusCode) return RedirectToAction("GetAllKM");
            return View();
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            var url = $"https://localhost:7095/api/KhuyenMai/{id}";
            var response = await _httpClient.DeleteAsync(url);
            if (response.IsSuccessStatusCode) return RedirectToAction("GetAllKM");
            return BadRequest();
        }
        // Get CTSP by Id Khuyen Mai
        // https://localhost:7095/api/KhuyenMai/GetCTSPByIdKm?idkm=03327098-cd91-4764-b905-358873673882
        public async Task<IActionResult> GetCTSPByIdKM(Guid id, int ProductPage = 1)
        {
            // list khuyen mai view
            string apiURL = $"https://localhost:7095/api/KhuyenMai";
            var response = await _httpClient.GetAsync(apiURL);
            var apiData = await response.Content.ReadAsStringAsync();
            var roles = JsonConvert.DeserializeObject<List<KhuyenMaiView>>(apiData);
            ViewBag.KhuyenMaiView = roles;
            // list AllCTSP by SP
            string apiURL1 = $"https://localhost:7095/api/KhuyenMai/GetCTSPByIdKm?idkm={id}";
            var response1 = await _httpClient.GetAsync(apiURL1);
            var apiData1 = await response1.Content.ReadAsStringAsync();
            var bienthes = JsonConvert.DeserializeObject<List<AllViewCTSP>>(apiData1);
            return View(new PhanTrangCTSPBySP
            {
                listallctspbysp = bienthes
                        .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = bienthes.Count()
                }
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetSPByIdKM(Guid id, int ProductPage = 1)
        {
            // list khuyen mai view
            string apiURL = $"https://localhost:7095/api/KhuyenMai";
            var response = await _httpClient.GetAsync(apiURL);
            var apiData = await response.Content.ReadAsStringAsync();
            var roles = JsonConvert.DeserializeObject<List<KhuyenMaiView>>(apiData);
            ViewBag.KhuyenMaiView = roles;
            // list AllCTSP by SP
            // lưu Id Khuyến Mãi Vô Session 
            var KhuyenMai = roles.FirstOrDefault(x=>x.ID==id);

            HttpContext.Session.SetString("IdKhuyenMai", KhuyenMai.ID.ToString());
            string apiURL1 = $"https://localhost:7095/api/KhuyenMai/GetAllSPByKhuyenMai?idkm={id}";
            var response1 = await _httpClient.GetAsync(apiURL1);
            var apiData1 = await response1.Content.ReadAsStringAsync();
            var bienthes = JsonConvert.DeserializeObject<List<AllViewSp>>(apiData1);
           
            
           
           
            return View(new PhanTrangAllQLKMSP
            {
                listallsp = bienthes
                        .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = bienthes.Count()
                }
            });
           
        }
        [HttpGet]
        public async Task<IActionResult> GetSPNoKM(int ProductPage = 1)
        {
            var idkhuyenmai = Guid.Parse(HttpContext.Session.GetString("IdKhuyenMai"));
            // // list AllViewsp
            string apiURL1 = $"https://localhost:7095/api/KhuyenMai/GetAllSPNoKM?id={idkhuyenmai}";
            var response1 = await _httpClient.GetAsync(apiURL1);
            var apiData1 = await response1.Content.ReadAsStringAsync();
            var qlsanphams = JsonConvert.DeserializeObject<List<AllViewSp>>(apiData1);
            return View(new PhanTrangAllQLKMSP
            {
                listallsp = qlsanphams.Where(x=>x.IdKhuyenMai!=idkhuyenmai)
                        .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = qlsanphams.Count()
                }

            });
        }
        //https://localhost:7095/api/KhuyenMai/GetAllSPNoKMByLoaiSPChatLieu?id=a034b1a2-1f42-43f2-b87a-138f8722cdcb&idLoaiSP=49ed4761-81c4-4f7d-a5c6-b051ed1ecdb6&idChatLieu=276b52d1-ce55-4027-b185-f6d5db4017b3
        [HttpGet]
        public async Task<IActionResult> GetSPNoKMLoaiSPCL(Guid? idloaisp, Guid? idchatlieu, int ProductPage = 1)
        {
            var idkhuyenmai = Guid.Parse(HttpContext.Session.GetString("IdKhuyenMai"));
            // // list AllViewsp
            string apiURL1 = $"https://localhost:7095/api/KhuyenMai/GetAllSPNoKMByLoaiSPChatLieu?id={idkhuyenmai}&idLoaiSP={idloaisp}&idChatLieu={idchatlieu}";
            var response1 = await _httpClient.GetAsync(apiURL1);
            var apiData1 = await response1.Content.ReadAsStringAsync();
            var qlsanphams = JsonConvert.DeserializeObject<List<AllViewSp>>(apiData1);
            return View("GetSPNoKM",new PhanTrangAllQLKMSP
            {
                listallsp = qlsanphams.Where(x => x.IdKhuyenMai != idkhuyenmai)
                        .Skip((ProductPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = ProductPage,
                    TotalItems = qlsanphams.Count()
                }

            });
        }

        #endregion
    }
}
