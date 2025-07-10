using System.Collections.Generic;
using System.Linq;

namespace PlantManagement.Models
{
    public static class Database
    {
        // Danh sách giả lập các tài khoản người dùng
        private static List<User> _users = new List<User>
        {
            new User { UserName = "admin", Email = "admin@example.com", Password = "123456", FullName = "Admin User" },
            new User { UserName = "user1", Email = "user1@example.com", Password = "password123", FullName = "User One" }
        };

        // Phương thức tìm kiếm tài khoản theo tên đăng nhập và email
        public static User FindUserByUsernameAndEmail(string username, string email)
        {
            return _users.FirstOrDefault(u => u.UserName == username && u.Email == email);
        }
    }
}
