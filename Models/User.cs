using System;

public class User
{
    public int ID { get; set; } // Khóa chính
    public string UserName { get; set; } // Tên đăng nhập
    public string FullName { get; set; } // Họ và tên
    public string Password { get; set; } // Mật khẩu
    public string Email { get; set; } // Email
    public bool IsActive { get; set; } // Trạng thái kích hoạt
    public DateTime CreatedAt { get; set; } // Ngày tạo tài khoản
    public int ID_Role { get; set; } // ID nhóm người dùng

    public string RoleName { get; set; } // Thêm GroupName để lưu tên nhóm
}
