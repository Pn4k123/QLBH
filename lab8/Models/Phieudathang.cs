using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace buoi5.Models
{
    public partial class Phieudathang
    {
        public Phieudathang()
        {
            Chitietphieudathang = new HashSet<Chitietphieudathang>();
            Phieugiaohang = new HashSet<Phieugiaohang>();
        }
        [DisplayName("Ma phieu")]
        public string Mapdh { get; set; }
        [DisplayName("Ngay dat hang")]
        public DateTime? Ngaydh { get; set; }
        [DisplayName("Ngay giao hang")]
        public DateTime? Ngaygh { get; set; }
        [DisplayName("Dia chi giao")]
        public string Diachigh { get; set; }
        public string Makh { get; set; }

        public Khachhang MakhNavigation { get; set; }
        public ICollection<Chitietphieudathang> Chitietphieudathang { get; set; }
        public ICollection<Phieugiaohang> Phieugiaohang { get; set; }
    }
}
