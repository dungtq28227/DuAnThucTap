using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Models
{
    public class NhanVien
    {
        public Guid ID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Tên.")]
        public string Ten { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Email.")]
        [EmailAddress]

        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Password.")]
        public string PassWord { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại.")]
        public string? SDT { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Địa chỉ.")]
        public string? DiaChi { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Trạng thái.")]
        public int? TrangThai { get; set; }
        public Guid IDVaiTro { get; set; }
        public virtual IEnumerable<HoaDon>? HoaDons { get; set; }
        public virtual VaiTro? VaiTro { get; set; }
    }
}
