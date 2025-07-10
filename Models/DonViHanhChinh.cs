public class DonViHanhChinh
{
    public int ID { get; set; } // Khóa chính
    public string TenDVHC { get; set; } // Tên đơn vị hành chính
    public int? ParentID { get; set; } // ID đơn vị hành chính cấp trên (nếu có)
    public int ID_Cap { get; set; } // ID cấp hành chính
}
