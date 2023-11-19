using AppData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace AppData.Configurations
{
    public class QuyDoiDiemConfiguration : IEntityTypeConfiguration<QuyDoiDiem>
    {
        public void Configure(EntityTypeBuilder<QuyDoiDiem> builder)
        {
            builder.ToTable("QuyDoiDiem");
            builder.HasKey(x => x.ID);
            builder.Property(x => x.SoDiem).HasColumnType("int");
            builder.Property(x => x.TiLeTichDiem).HasColumnType("int");
            builder.Property(x => x.TiLeTieuDiem).HasColumnType("int");
            builder.Property(x => x.TrangThai).HasColumnType("int");
        }
    }
}
