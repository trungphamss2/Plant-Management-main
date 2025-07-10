using System;
using System.Data.SqlClient;
using System.Data;
using PlantManagement.Models;
using PlantManagement.Helpers;
using System.Collections.Generic;

namespace PlantManagement.Controllers
{
    public class QuanLyThongTinTaiKhoanController
    {
        private DatabaseHelper _dbHelper;

        public QuanLyThongTinTaiKhoanController()
        {
            _dbHelper = new DatabaseHelper();
        }

        // Lấy thông tin người dùng từ cơ sở dữ liệu
        public User GetUserInfo(int userId)
        {
            string query = @"
                SELECT u.ID, u.FullName, u.UserName, u.Email, u.ID_Role, u.CreatedAt 
                FROM [User] u
                WHERE u.ID = @UserID";

            var parameters = new[] {
                new SqlParameter("@UserID", SqlDbType.Int) { Value = userId }
            };

            DataTable userData = _dbHelper.ExecuteQuery(query, parameters);
            if (userData.Rows.Count > 0)
            {
                DataRow row = userData.Rows[0];
                int roleId = (int)row["ID_Role"]; // Lấy ID_Role của người dùng

                // Lấy RoleName từ ID_Role
                string roleName = GetRoleNameById(roleId);

                return new User
                {
                    ID = (int)row["ID"],
                    FullName = (string)row["FullName"],
                    UserName = (string)row["UserName"],
                    Email = (string)row["Email"],
                    CreatedAt = (DateTime)row["CreatedAt"],
                    // Chỉ giữ lại ID_Role và không cần RoleName trong User
                };
            }

            return null;
        }

        public int GetIdRoleByName(string roleName)
        {
            string query = "SELECT ID FROM Role WHERE RoleName = @RoleName";
            var parameters = new SqlParameter[] {
        new SqlParameter("@RoleName", SqlDbType.NVarChar) { Value = roleName }
    };

            // Giả sử ExecuteScalar sẽ trả về kết quả đầu tiên (ID của role)
            return (int)_dbHelper.ExecuteScalar(query, parameters);
        }


        // Lấy RoleName từ ID_Role
        // Phương thức trong QuanLyThongTinTaiKhoanController để lấy RoleName từ ID_Role
        public string GetRoleNameById(int roleId)
        {
            string query = "SELECT RoleName FROM Role WHERE ID = @RoleID";
            var parameters = new SqlParameter[] {
        new SqlParameter("@RoleID", SqlDbType.Int) { Value = roleId }
    };

            return (string)_dbHelper.ExecuteScalar(query, parameters);
        }


        // Cập nhật thông tin người dùng trong cơ sở dữ liệu
        public bool UpdateUserInfo(User user, string roleName)
        {
            // Lấy ID của Role từ tên Role
            string query = "SELECT ID FROM Role WHERE RoleName = @RoleName";
            var roleParameter = new SqlParameter("@RoleName", SqlDbType.NVarChar) { Value = roleName };

            // Tham số cần truyền vào là mảng
            SqlParameter[] parameters = new SqlParameter[] { roleParameter };

            // Lấy ID của Role từ cơ sở dữ liệu
            int roleId = (int)_dbHelper.ExecuteScalar(query, parameters);

            // Cập nhật thông tin người dùng
            query = @"
        UPDATE [User]
        SET FullName = @FullName, Email = @Email, ID_Role = @RoleID
        WHERE ID = @UserID";

            var userParameters = new[] {
        new SqlParameter("@FullName", SqlDbType.NVarChar) { Value = user.FullName },
        new SqlParameter("@Email", SqlDbType.NVarChar) { Value = user.Email },
        new SqlParameter("@RoleID", SqlDbType.Int) { Value = roleId },  // Sử dụng ID của Role
        new SqlParameter("@UserID", SqlDbType.Int) { Value = user.ID }
    };

            int rowsAffected = _dbHelper.ExecuteNonQuery(query, userParameters); // Đổi thành userParameters
            return rowsAffected > 0;
        }


        // Lấy danh sách các vai trò từ cơ sở dữ liệu
        public List<string> GetRoleNames()
        {
            string query = "SELECT RoleName FROM Role";
            var roles = _dbHelper.ExecuteQuery(query);

            List<string> roleList = new List<string>();
            foreach (DataRow row in roles.Rows)
            {
                roleList.Add(row.Field<string>("RoleName"));
            }

            return roleList;
        }
    }
}
