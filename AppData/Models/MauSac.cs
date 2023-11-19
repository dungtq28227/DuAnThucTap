﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Models
{
    public class MauSac
    {
        public Guid? ID { get; set; }
        public string? Ten { get; set; }
        public string? Ma { get; set; }
        public int? TrangThai { get; set; }
        public virtual IEnumerable<ChiTietSanPham>? ChiTietSanPhams { get; set; }
        public virtual IEnumerable<Anh>? Anhs { get; set; }
    }
}
