using AppData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Configurations
{
    internal class NhanVienConfiguration : IEntityTypeConfiguration<NhanVien>
    {
        public void Configure(EntityTypeBuilder<NhanVien> builder)
        {
            builder.ToTable("NhanVien");
            builder.HasKey(x => x.ID);
            builder.Property(x => x.Ten).HasColumnType("nvarchar(20)").IsRequired();
            builder.Property(x => x.Email).HasColumnType("nvarchar(20)").IsRequired();
            builder.Property(x => x.SDT).HasColumnType("nvarchar(20)").IsRequired();
            builder.Property(x => x.DiaChi).HasColumnType("nvarchar(20)").IsRequired();
            builder.Property(x => x.TrangThai).HasColumnType("int");
            builder.HasOne(x => x.VaiTro).WithMany(x => x.NhanViens).HasForeignKey(x => x.IDVaiTro);
        }
    }
}
