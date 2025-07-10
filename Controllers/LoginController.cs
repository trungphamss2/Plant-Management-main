using PlantManagement.Helpers;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System;
using System.Net;

public class LoginController
{
    private readonly DatabaseHelper _dbHelper;

    public LoginController()
    {
        _dbHelper = new DatabaseHelper(); // Khởi tạo DatabaseHelper
    }

    // Phương thức xác thực người dùng (Đăng nhập)
    public User AuthenticateUser(string username, string password)
    {
        const string query = @"
        SELECT ID, UserName, FullName, [Password], Email, IsActive, CreatedAt, ID_Role
        FROM [User] 
        WHERE UserName = @UserName";

        var parameters = new[]
        {
            new SqlParameter("@UserName", SqlDbType.NVarChar) { Value = username }
        };

        var dataTable = _dbHelper.ExecuteQuery(query, parameters);

        if (dataTable.Rows.Count > 0)
        {
            var row = dataTable.Rows[0];
            var user = new User
            {
                ID = Convert.ToInt32(row["ID"]),
                UserName = row["UserName"].ToString(),
                FullName = row["FullName"].ToString(),
                Password = row["Password"].ToString(),
                Email = row["Email"].ToString(),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                ID_Role = Convert.ToInt32(row["ID_Role"]) // Đổi từ ID_Group sang ID_Role
            };

            // Kiểm tra mật khẩu và trạng thái người dùng
            if (user.IsActive && password == user.Password) // Cần mã hóa mật khẩu trong thực tế
            {
                return user; // Trả về đối tượng User nếu đăng nhập thành công
            }
        }

        return null; // Trả về null nếu không thành công
    }

    // Phương thức lưu thông tin đăng nhập vào bảng LoginHistory
    public void SaveLoginHistory(int userId, bool isSuccessful)
    {
        string ipAddress = GetClientIpAddress(); // Lấy địa chỉ IP của người dùng

        const string query = @"
    INSERT INTO LoginHistory (UserID, LoginTime, IPAddress)
    VALUES (@UserID, @LoginTime, @IPAddress)";

        var parameters = new[]
        {
        new SqlParameter("@UserID", SqlDbType.Int) { Value = userId },
        new SqlParameter("@LoginTime", SqlDbType.DateTime) { Value = DateTime.Now },
        new SqlParameter("@IPAddress", SqlDbType.NVarChar) { Value = ipAddress }
    };

        _dbHelper.ExecuteNonQuery(query, parameters);
    }

    // Phương thức lấy địa chỉ IP của người dùng
    private string GetClientIpAddress()
    {
        string ipAddress = "127.0.0.1"; // Default to localhost in case of error

        try
        {
            ipAddress = Dns.GetHostEntry(Dns.GetHostName())
                          .AddressList.FirstOrDefault(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                          ?.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return ipAddress;
    }

    // Phương thức lấy mật khẩu theo username và email (cho chức năng quên mật khẩu)
    public string GetPasswordByUsernameAndEmail(string username, string email)
    {
        const string query = @"
            SELECT [Password]
            FROM [User]
            WHERE UserName = @UserName AND Email = @Email";

        var parameters = new[]
        {
            new SqlParameter("@UserName", SqlDbType.NVarChar) { Value = username },
            new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email }
        };

        var dataTable = _dbHelper.ExecuteQuery(query, parameters);

        if (dataTable.Rows.Count > 0)
        {
            return dataTable.Rows[0]["Password"].ToString();  // Trả về mật khẩu
        }

        return null;  // Không tìm thấy người dùng hoặc email không đúng
    }
}
