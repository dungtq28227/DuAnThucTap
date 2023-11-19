using AppAPI.IServices;
using AppData.IRepositories;
using AppData.Models;
using AppData.Repositories;
using AppData.ViewModels;
using AppData.ViewModels.QLND;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace AppAPI.Services
{
    public class QuanLyNguoiDungService : IQuanLyNguoiDungService
    {
        private readonly IAllRepository<NhanVien> reposNV;
        private readonly IAllRepository<KhachHang> reposKH;
        AssignmentDBContext context = new AssignmentDBContext();

        public QuanLyNguoiDungService()
        {
            reposNV = new AllRepository<NhanVien>(context, context.NhanViens);
            reposKH = new AllRepository<KhachHang>(context, context.KhachHangs);
        }
        public async Task<bool> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            bool isEmployee = await CheckIfEmployee(email);
            if (isEmployee)
            {
                string resetToken = GenerateToken();
                await SaveUserData(email, resetToken, isEmployee);
                string subject = "Reset Password";
                string messageBody = "You have requested a password reset. Your reset token is: " + resetToken;
                await SendEmail(email, subject, messageBody);

                return true;
            }
            return false;
        }
        public async Task<bool> ResetPassword(ResetPasswordRequest model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return false;
            }
            bool isEmployee = await CheckIfEmployee(model.Email);

            if (isEmployee)
            {
                var nv = await context.NhanViens.FirstOrDefaultAsync(a => a.Email == model.Email);
                if (nv != null)
                {
                    nv.PassWord = model.Password;
                    await context.SaveChangesAsync();
                    await SendEmail(nv.Email, "Đổi Mật Khẩu Thành Công", "Mật khẩu của bạn đã được đặt lại thành công.");

                    return true;
                }
            }
            else
            {
                var kh = await context.KhachHangs.FirstOrDefaultAsync(a => a.Email == model.Email);
                if (kh != null)
                {
                    kh.Password = model.Password;

                    await context.SaveChangesAsync();
                    await SendEmail(kh.Email, "Đặt lại mật khẩu thành công", "Mật khẩu của bạn đã được đặt lại thành công.");
                    return true;
                }
            }
            await SendEmail(model.Email, "Lỗi Đặt lại Mật khẩu", "Đã xảy ra lỗi khi đặt lại mật khẩu của bạn. Vui lòng thử lại sau.");
            return false;
        }
        private string GenerateToken()
        {
            string token = Guid.NewGuid().ToString();
            return token;
        }

        public async Task<bool> CheckIfEmployee(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            var nv = await context.NhanViens.FirstOrDefaultAsync(a => a.Email == email);
            var kh = await context.KhachHangs.FirstOrDefaultAsync(a => a.Email == email);
            return nv != null || kh != null;
        }
        private async Task<bool> SaveUserData(string email, string data, bool isEmployee)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            if (isEmployee)
            {
                var nv = await context.NhanViens.FirstOrDefaultAsync(a => a.Email == email);
                if (nv != null)
                {
                    nv.PassWord = data;
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            else
            {
                var kh = await context.KhachHangs.FirstOrDefaultAsync(a => a.Email == email);
                if (kh != null)
                {
                    kh.Password = data;
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
        private async Task<bool> SendEmail(string email, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com", 465);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential("nhu3006a12@gmail.com", "nhucong.");
                var messsage = new MailMessage();
                messsage.From = new MailAddress("nhu3006a12@gmail.com");
                messsage.To.Add(new MailAddress(email));
                messsage.Subject = subject;
                messsage.Body = body;
                await smtpClient.SendMailAsync(messsage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> ChangePassword(string email, string password, string newPassword)
        {
            var kh = await context.KhachHangs.FirstOrDefaultAsync(h => h.Email == email && h.Password == password);
            if (kh != null)
            {
                kh.Password = newPassword;
                await context.SaveChangesAsync();
                return true;
            }
            var nv = await context.NhanViens.FirstOrDefaultAsync(h => h.Email == email && h.PassWord == password);
            if (nv != null)
            {
                nv.PassWord = newPassword;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<LoginViewModel> Login(string lg, string password)
        {
            var nv = await  context.NhanViens.FirstOrDefaultAsync(a => (a.Email == lg || a.SDT == lg) && a.PassWord == password);
            if(nv != null)
            {
                return new LoginViewModel 
                { 
                    Id = nv.ID,
                    Email = nv.Email, 
                    Ten = nv.Ten,
                    SDT = nv.SDT,
                    vaiTro = 0
                };
            }
            var kh = await context.KhachHangs.FirstOrDefaultAsync(x => (x.Email == lg || x.SDT == lg) && x.Password == password);
            if(kh != null)
            {
                return new LoginViewModel
                {
                    Id = kh.IDKhachHang,
                    Email = kh.Email,
                    Ten = kh.Ten,
                    SDT = kh.SDT,
                    DiemTich = kh.DiemTich,
                    vaiTro = 1
                };
            }
            return null;
        }

        //public async Task<object> Login(string email, string password)
        //{
        //    var nv = await context.NhanViens.FirstOrDefaultAsync(a => a.Email == email && a.PassWord == password);
        //    if (nv != null)
        //    {
        //        return nv;
        //    }
        //    var kh = await context.KhachHangs.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
        //    if (kh != null)
        //    {
        //        return kh;
        //    }
        //    return null;
        //}

        public async Task<KhachHang> RegisterKhachHang(KhachHangViewModel khachHang)
        {
            KhachHang kh = new KhachHang()
            {
                IDKhachHang = Guid.NewGuid(),
                Ten = khachHang.Name,
                Email = khachHang.Email,
                Password = khachHang.Password
            };
            await context.KhachHangs.AddAsync(kh);
            //await context.SaveChangesAsync();
            GioHang gioHang = new GioHang()
            {
                IDKhachHang = kh.IDKhachHang,
                NgayTao = DateTime.Now,
            };
            await context.GioHangs.AddAsync(gioHang);
            await context.SaveChangesAsync();
            return kh;
        }

        public async Task<NhanVien> RegisterNhanVien(NhanVienViewModel nhanVien)
        {
            var kh = new NhanVien
            {
                ID = Guid.NewGuid(),
                Ten = nhanVien.Name,
                Email = nhanVien.Email,
                PassWord = nhanVien.Password,
                IDVaiTro = nhanVien.IDVaiTro
            };
            context.NhanViens.Add(kh);
            await context.SaveChangesAsync();
            return kh;
        }
        //Tam
        public async Task<bool> ChangePassword(ChangePasswordRequest request)
        {
            var kh = await context.KhachHangs.FirstOrDefaultAsync(h => h.IDKhachHang == request.ID);
            if (kh != null)
            {
                kh.Password = request.NewPassword;
                await context.SaveChangesAsync();
                return true;
            }
            var nv = await context.NhanViens.FirstOrDefaultAsync(h => h.ID == request.ID);
            if (nv != null)
            {
                nv.PassWord = request.NewPassword;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        //End
    }
}
