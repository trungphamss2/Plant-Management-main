using System.Data;
using System.Data.SqlClient;
using PlantManagement.Helpers;

namespace PlantManagement.Controllers
{
    public class AuthController
    {
        private readonly DatabaseHelper _dbHelper;

        public AuthController()
        {
            _dbHelper = new DatabaseHelper();
        }

        /// <summary>
        /// Lấy lại mật khẩu dựa trên tên đăng nhập và email.
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="email">Email</param>
        /// <returns>Mật khẩu nếu tìm thấy, ngược lại trả về null</returns>
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
                return dataTable.Rows[0]["Password"].ToString();
            }

            return null;
        }
    }
}
