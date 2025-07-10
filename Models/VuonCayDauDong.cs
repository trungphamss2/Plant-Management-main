public class VuonCayDauDong
{
    public int ID { get; set; } // Khóa chính
    public int ID_GiongCayTrong { get; set; } // ID giống cây trồng
    public string TenVuonCay { get; set; } // Tên vườn cây đầu dòng
    public int? ID_DVHC { get; set; } // ID đơn vị hành chính
}
