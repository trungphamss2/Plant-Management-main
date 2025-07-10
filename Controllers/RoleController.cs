using PlantManagement.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantManagement.Controllers
{
    public class RoleController
    {
        private readonly DatabaseHelper _dbHelper;
        public RoleController()
        {
            _dbHelper = new DatabaseHelper();
        }
        public string GetRoleUser(string username)
        {
            string query = "SELECT r.RoleName FROM [User] u JOIN [Role] r ON u.ID = r.ID WHERE u.Username = @Username";
            var parameter = new SqlParameter("@Username", SqlDbType.NVarChar) { Value = username };

            var role = _dbHelper.ExecuteScalar(query, new SqlParameter[] { parameter });
            return role?.ToString();
        }
    }
}