public class SXTT
{
    public int ID { get; set; } // Khóa chính
    public bool CSAnToanVietGap { get; set; } // Cơ sở sản xuất đạt tiêu chuẩn VietGAP
    public string VungTrong { get; set; } // Vị trí vùng trồng (tọa độ địa lý)
    public string SinhVatGayHai { get; set; } // Sinh vật gây hại
    public int ID_DVHC { get; set; } // ID đơn vị hành chính
}
