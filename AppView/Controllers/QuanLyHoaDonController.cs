using AppData.ViewModels.BanOffline;
using Microsoft.AspNetCore.Mvc;

namespace AppView.Controllers
{
    public class QuanLyHoaDonController : Controller
    {
        private readonly HttpClient _httpClient;

        public QuanLyHoaDonController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7095/api/");
        }
        //View QLHD
        public IActionResult _QuanLyHoaDon()
        {
            return View();
        }
        // Load tất cả hóa đơn
        [HttpGet]
        public async Task<IActionResult> LoadAllHoaDon(int page, int pagesize)
        {
            var listhdql = await _httpClient.GetFromJsonAsync<List<HoaDonQL>>("HoaDon/GetAllHDQly");
            listhdql = listhdql.OrderByDescending(c => c.ThoiGian).ToList();
            int totalRow = listhdql.Count;
            var model = listhdql.Skip((page - 1) * pagesize).Take(pagesize).ToArray();
            return Json(new
            {
                data = model,
                total = totalRow,
                status = true,
            });
        }
        //Chi tiết hóa đơn 
        [HttpGet("/QuanLyHoaDon/ViewChiTietHD/{idhd}")]
        public async Task<IActionResult> ViewChiTietHD(string idhd)
        {
            var hd = await _httpClient.GetFromJsonAsync<ChiTietHoaDonQL>($"HoaDon/ChiTietHoaDonQL/{idhd}");
            return PartialView("_ThongTinHD", hd);
        }
        //Lọc HD 
        public async Task<IActionResult> LocHD(FilterHD filter)
        {
            //(hd.TrangThaiGiaoHang == 2 ? "Chờ bàn giao" :
            //                            (hd.TrangThaiGiaoHang == 3 ? "Đang giao" :
            //                            (hd.TrangThaiGiaoHang == 4 ? "Đang hoàn hàng" :
            //                            (hd.TrangThaiGiaoHang == 5 ? "Hoàn hàng thành công" :
            //                            (hd.TrangThaiGiaoHang == 6 ? "Thành công" :
            //                            (hd.TrangThaiGiaoHang == 7 ? "Đơn hủy" :
            //                            (hd.TrangThaiGiaoHang == 8 ? "Đơn hàng thất lạc" : "Khác"))))))),
            var listhdql = await _httpClient.GetFromJsonAsync<List<HoaDonQL>>("HoaDon/GetAllHDQly");
            listhdql = listhdql.OrderByDescending(c => c.ThoiGian).ToList();
            //Lọc loại hd
            if(filter.loaiHD != null)
            {
                listhdql = listhdql.Where(c => filter.loaiHD.Contains(c.LoaiHD)).ToList();
            }
            //Lọc trạng thái
            if(filter.lstTT != null)
            {
                listhdql = listhdql.Where(c => filter.lstTT.Contains(c.TrangThai)).ToList();
            }
            int totalRow = listhdql.Count;
            var model = listhdql.Skip((filter.page - 1) * filter.pageSize).Take(filter.pageSize).ToArray();
            return Json(new
            {
                data = model,
                total = totalRow,
                status = true,
            });
        }
    }
}
