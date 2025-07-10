using System;
using System.Data;
using System.Data.SqlClient;

namespace PlantManagement.Helpers
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper()
        {
            // Chuỗi kết nối tới cơ sở dữ liệu
            _connectionString = "Server=DESKTOP-ICACP6G\\SQLEXPRESS;Database=QuanLyTrongTrot;Trusted_Connection=True;";
        }

        // Phương thức mở kết nối
        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Phương thức thực thi truy vấn trả về một giá trị (ExecuteScalar)
        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (var connection = GetConnection())
            {
                var command = new SqlCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                object result = command.ExecuteScalar();
                connection.Close();

                return result;
            }
        }

        // Phương thức thực thi truy vấn không trả về (ExecuteNonQuery)
        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (var connection = GetConnection())
            {
                var command = new SqlCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                return rowsAffected;
            }
        }

        // Phương thức thực thi truy vấn trả về DataTable (ExecuteQuery)
        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (var connection = GetConnection())
            {
                var command = new SqlCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                var dataTable = new DataTable();
                var adapter = new SqlDataAdapter(command);

                connection.Open();
                adapter.Fill(dataTable);
                connection.Close();

                return dataTable;
            }
        }
    }
}
