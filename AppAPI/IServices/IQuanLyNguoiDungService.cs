using AppData.Models;
using AppData.ViewModels;
using AppData.ViewModels.QLND;

namespace AppAPI.IServices
{
    public interface IQuanLyNguoiDungService
    {
        Task<LoginViewModel> Login(string lg, string password);
        Task<KhachHang> RegisterKhachHang(KhachHangViewModel khachHang);
        Task<NhanVien> RegisterNhanVien(NhanVienViewModel nhanVien);
        Task<bool> ChangePassword(string email, string password, string newPassword);
        Task<bool> ChangePassword(ChangePasswordRequest request);
        Task<bool> ForgetPassword(string email);

        Task<bool> ResetPassword(ResetPasswordRequest model);
    }
}
