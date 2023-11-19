using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.ViewModels
{
    public class FilterData
    {
        public List<string> priceRange { get; set; }
        public string search { get; set; }
        public List<string> loaiSP { get; set; }
        public List<Guid> mauSac { get; set; }
        public List<Guid> kichCo { get; set; }
        public List<Guid> chatLieu { get; set; }
        public string sortSP { get; set; }
    }
}
