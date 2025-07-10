using System;
using System.Data;
using System.Data.SqlClient;
using PlantManagement.Helpers;

namespace PlantManagement.Controllers
{
    public class RegisterController
    {
        private readonly DatabaseHelper _dbHelper;

        public RegisterController()
        {
            _dbHelper = new DatabaseHelper();
        }

        public bool Register(string username, string fullName, string email, string password)
        {
            try
            {
                // Kiểm tra trùng lặp username
                const string checkQuery = "SELECT COUNT(*) FROM [User] WHERE UserName = @UserName";
                var checkParams = new[] { new SqlParameter("@UserName", SqlDbType.NVarChar) { Value = username } };
                int count = (int)_dbHelper.ExecuteScalar(checkQuery, checkParams);

                if (count > 0)
                {
                    // Username đã tồn tại
                    return false;
                }

                // Thêm người dùng mới vào cơ sở dữ liệu
                const string insertQuery = @"
                    INSERT INTO [User] (UserName, FullName, Email, [Password], IsActive, CreatedAt, ID_Role) 
                    VALUES (@UserName, @FullName, @Email, @Password, 1, GETDATE(), 2)";

                var parameters = new[]
                {
                    new SqlParameter("@UserName", SqlDbType.NVarChar) { Value = username },
                    new SqlParameter("@FullName", SqlDbType.NVarChar) { Value = fullName },
                    new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email },
                    new SqlParameter("@Password", SqlDbType.NVarChar) { Value = password } // Nên mã hóa mật khẩu
                };

                int rowsAffected = _dbHelper.ExecuteNonQuery(insertQuery, parameters);
                return rowsAffected > 0; // Thành công nếu số dòng ảnh hưởng > 0
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (log, thông báo, ...)
                Console.WriteLine($"Error while registering: {ex.Message}");
                return false;
            }
        }
    }
}
