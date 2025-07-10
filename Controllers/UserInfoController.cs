using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlantManagement.Helpers;

namespace PlantManagement.Controllers
{
    internal class UserInfoController
    {
        private DatabaseHelper _dbHelper;

        public UserInfoController()
        {
            _dbHelper = new DatabaseHelper();
        }

        // Lấy danh sách người dùng
        public List<User> GetUserList()
        {
            var userList = new List<User>();

            try
            {
                string query = @"
            SELECT 
                u.ID, u.UserName, u.FullName, u.Email, u.IsActive, u.CreatedAt, u.ID_Role, ug.RoleName, u.Password    
            FROM [User] u
            LEFT JOIN Role ug ON u.ID_Role = ug.ID
            ORDER BY u.UserName";

                using (var dataTable = _dbHelper.ExecuteQuery(query))
                {
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow row in dataTable.Rows)
                        {
                            var user = new User
                            {
                                ID = Convert.ToInt32(row["ID"]),
                                UserName = row["UserName"].ToString(),
                                FullName = row["FullName"].ToString(),
                                Email = row["Email"].ToString(),
                                IsActive = Convert.ToBoolean(row["IsActive"]),
                                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                                ID_Role = Convert.ToInt32(row["ID_Role"]),
                                Password = row["Password"].ToString()
                            };

                            // Thêm RoleName từ bảng Role
                            user.RoleName = row["RoleName"].ToString();

                            userList.Add(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user list: {ex.Message}");
            }

            return userList;
        }
        public List<LoginHistory> GetLoginHistoryWithFullName()
        {
            var loginHistoryList = new List<LoginHistory>();

            try
            {
                string query = @"
            SELECT 
                lh.ID, 
                lh.UserID, 
                u.FullName, 
                lh.LoginTime, 
                lh.IPAddress
            FROM LoginHistory lh
            INNER JOIN [User] u ON lh.UserID = u.ID
            ORDER BY lh.LoginTime DESC";

                using (var dataTable = _dbHelper.ExecuteQuery(query))
                {
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow row in dataTable.Rows)
                        {
                            var loginHistory = new LoginHistory
                            {
                                ID = Convert.ToInt32(row["ID"]),
                                UserID = Convert.ToInt32(row["UserID"]),
                                FullName = row["FullName"].ToString(),  // Thêm FullName
                                LoginTime = Convert.ToDateTime(row["LoginTime"]),
                                IPAddress = row["IPAddress"].ToString()
                            };

                            loginHistoryList.Add(loginHistory);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching login history: {ex.Message}");
            }

            return loginHistoryList;
        }
        public DataTable GetLoginHistoryData()
        {
            DataTable loginHistoryTable = new DataTable();
            loginHistoryTable.Columns.Add("UserName", typeof(string));
            loginHistoryTable.Columns.Add("LoginCount", typeof(int));

            try
            {
                // Truy vấn bảng User và số lần đăng nhập từ bảng LoginHistory
                string query = @"
            SELECT u.UserName, COUNT(LH.ID) AS LoginCount
            FROM [User] u
            LEFT JOIN LoginHistory LH ON u.ID = LH.UserID
            GROUP BY u.UserName
            ORDER BY LoginCount DESC;";

                var dataTable = _dbHelper.ExecuteQuery(query);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        DataRow newRow = loginHistoryTable.NewRow();
                        newRow["UserName"] = row["UserName"];
                        newRow["LoginCount"] = row["LoginCount"];
                        loginHistoryTable.Rows.Add(newRow);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching login history: {ex.Message}");
            }

            return loginHistoryTable;
        }


        public DataTable GetPermissionData()
        {
            DataTable permissionTable = new DataTable();

            try
            {
                string query = @"
SELECT 
    p.ID AS ID_Permission,
    p.PermissionName,
    r.ID AS ID_Role,
    r.RoleName
FROM Permission p
LEFT JOIN Role_Permission rp ON p.ID = rp.ID_Permission
LEFT JOIN [Role] r ON rp.ID_Role = r.ID
ORDER BY p.PermissionName, r.RoleName;";

                permissionTable = _dbHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching permission data: {ex.Message}");
            }

            return permissionTable;
        }

        public bool DeleteUser(int userId)
        {
            try
            {
                string query = "DELETE FROM [User] WHERE ID = @UserID";

                // Chuyển đổi Dictionary thành mảng SqlParameter
                var parameters = new SqlParameter[]
                {
            new SqlParameter("@UserID", userId)
                };

                int rowsAffected = _dbHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0; // Trả về true nếu thao tác xóa thành công
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                return false;
            }
        }

        public bool UpdateUser(User updatedUser)
        {
            try
            {
                string query = @"
        UPDATE [User]
        SET 
            UserName = @UserName,
            FullName = @FullName,
            Email = @Email,
            IsActive = @IsActive,
            ID_Role = @ID_Role
        WHERE ID = @ID";

                // Chuyển đổi Dictionary<string, object> thành SqlParameter[]
                var parameters = new[]
{
    new SqlParameter("@ID", updatedUser.ID),
    new SqlParameter("@UserName", updatedUser.UserName ?? (object)DBNull.Value),
    new SqlParameter("@FullName", updatedUser.FullName ?? (object)DBNull.Value),
    new SqlParameter("@Email", updatedUser.Email ?? (object)DBNull.Value),
    new SqlParameter("@IsActive", updatedUser.IsActive),
    new SqlParameter("@ID_Role", updatedUser.ID_Role)
};


                // Gọi phương thức ExecuteNonQuery
                return _dbHelper.ExecuteNonQuery(query, parameters) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return false;
            }
        }
        public bool AddNewUser(string userName, string fullName, string email, string password)
        {
            try
            {
                string query = @"
        INSERT INTO [User] (UserName, FullName, Email, Password, IsActive, CreatedAt)
        VALUES (@UserName, @FullName, @Email, @Password, 1, GETDATE())";

                var parameters = new[]
                {
            new SqlParameter("@UserName", userName),
            new SqlParameter("@FullName", fullName),
            new SqlParameter("@Email", email),
            new SqlParameter("@Password", password)
        };

                int rowsAffected = _dbHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding new user: {ex.Message}");
                return false;
            }
        }




    }
}