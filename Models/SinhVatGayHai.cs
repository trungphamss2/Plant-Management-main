using System;

public class SinhVatGayHai
{
    public int ID { get; set; } // Khóa chính
    public string TenSVGH { get; set; } // Tên sinh vật gây hại
    public double MatDo { get; set; } // Mật độ sinh vật gây hại
    public string DoPhoBien { get; set; } // Độ phổ biến (cao, thấp, khác,...)
    public double TuoiSau { get; set; } // Tuổi sinh vật gây hại (tháng,...)
    public string ViTri { get; set; } // Vị trí (dữ liệu địa lý, có thể lưu dưới dạng địa chỉ hoặc tọa độ)
}
