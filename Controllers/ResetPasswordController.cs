using System;
using System.Data;
using System.Data.SqlClient;
using PlantManagement.Helpers;

namespace PlantManagement.Controllers
{
    public class ResetPasswordController
    {
        private readonly DatabaseHelper _dbHelper;

        public ResetPasswordController()
        {
            // Khởi tạo đối tượng DatabaseHelper
            _dbHelper = new DatabaseHelper();
        }

        public bool UpdatePassword(int userId, string oldPassword, string newPassword)
        {
            try
            {
                // Truy vấn kiểm tra thông tin tài khoản và mật khẩu cũ
                const string queryCheck = @"
                SELECT [Password], IsActive
                FROM [User] 
                WHERE ID = @UserID";

                var parameters = new[]
                {
                    new SqlParameter("@UserID", SqlDbType.Int) { Value = userId }
                };

                var dataTable = _dbHelper.ExecuteQuery(queryCheck, parameters);

                if (dataTable.Rows.Count == 0)
                {
                    // Nếu không tìm thấy tài khoản
                    return false;
                }

                var row = dataTable.Rows[0];
                var userPassword = row["Password"].ToString();
                var isActive = Convert.ToBoolean(row["IsActive"]);

                // Kiểm tra trạng thái tài khoản và mật khẩu cũ
                if (!isActive)
                {
                    // Nếu tài khoản không hoạt động
                    return false;
                }

                if (oldPassword != userPassword)
                {
                    // Nếu mật khẩu cũ không khớp
                    return false;
                }

                // Thực hiện cập nhật mật khẩu mới
                const string queryUpdate = "UPDATE [User] SET [Password] = @NewPassword WHERE ID = @UserID";

                var updateParameters = new[]
                {
                    new SqlParameter("@NewPassword", SqlDbType.NVarChar) { Value = newPassword },
                    new SqlParameter("@UserID", SqlDbType.Int) { Value = userId }
                };

                int rowsAffected = _dbHelper.ExecuteNonQuery(queryUpdate, updateParameters);

                return rowsAffected > 0; // Trả về true nếu cập nhật thành công
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                Console.WriteLine($"Error updating password: {ex.Message}");
                return false;
            }
        }
    }
}
