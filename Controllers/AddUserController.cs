using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PlantManagement.Helpers;
using PlantManagement.Models;

namespace PlantManagement.Controllers
{
    public class AddUserController
    {
        private readonly DatabaseHelper _dbHelper;

        // Constructor để khởi tạo đối tượng DatabaseHelper
        public AddUserController()
        {
            _dbHelper = new DatabaseHelper();
        }

        // Phương thức lấy danh sách tên vai trò từ cơ sở dữ liệu
        public List<string> GetRoleNames()
        {
            List<string> roleNames = new List<string>();
            string query = "SELECT DISTINCT RoleName FROM Role";  // Sử dụng DISTINCT để chỉ lấy các vai trò không trùng lặp

            try
            {
                // Lấy dữ liệu từ cơ sở dữ liệu dưới dạng DataTable
                DataTable data = _dbHelper.ExecuteQuery(query, null);

                // Duyệt qua các dòng trong DataTable và lấy RoleName
                foreach (DataRow row in data.Rows)
                {
                    roleNames.Add(row["RoleName"].ToString()); // Lấy tên vai trò từ cột "RoleName"
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách vai trò: {ex.Message}");
            }

            return roleNames;
        }

        // Phương thức thêm người dùng mới vào cơ sở dữ liệu
        // Phương thức thêm người dùng mới vào cơ sở dữ liệu với ID_Role được truy vấn từ RoleName
        public bool AddUser(string username, string fullName, string email, string password, string roleName)
        {
            int roleId = 1;
            if (string.Equals(roleName, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                roleId = 1;
            }
            else if (string.Equals(roleName, "User", StringComparison.OrdinalIgnoreCase))
            {
                roleId = 2;
            }
            else
            {
                Console.WriteLine("Vai trò không hợp lệ." + roleId + " " + roleName);
                return false;
            }

            try
            {
                
                

                // Thêm người dùng mới vào cơ sở dữ liệu
                const string insertQuery = @"
                    INSERT INTO [User] (UserName, FullName,  [Password], Email, IsActive, CreatedAt, ID_Role) 
                    VALUES (@UserName, @FullName, @Email, @Password, 1, GETDATE(), @ID_Role)";

                var parameters = new[]
                {
                    new SqlParameter("@UserName", SqlDbType.NVarChar) { Value = username },
                    new SqlParameter("@FullName", SqlDbType.NVarChar) { Value = fullName },
                    new SqlParameter("@Password", SqlDbType.NVarChar) { Value = password }, // Nên mã hóa mật khẩu
                    new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email },
                    new SqlParameter("@ID_Role", SqlDbType.Int) { Value = roleId }
                };
                Console.WriteLine(roleId); 
                int rowsAffected = _dbHelper.ExecuteNonQuery(insertQuery, parameters);
                return rowsAffected > 0; // Thành công nếu số dòng ảnh hưởng > 0
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (log, thông báo, ...)
                Console.WriteLine($"Lỗi khi thêm người dùng: {ex.Message}");
                return false;
            }
        }


    }
}
