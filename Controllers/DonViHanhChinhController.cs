using System;
using System.Data;
using System.Data.SqlClient;
using PlantManagement.Helpers;

namespace PlantManagement.Controllers
{
    public class DonViHanhChinhController
    {
        private readonly DatabaseHelper _dbHelper;

        public DonViHanhChinhController()
        {
            _dbHelper = new DatabaseHelper();
        }

        // Lấy danh sách cấp hành chính
        public DataTable GetDanhMucCap()
        {
            try
            {
                string query = "SELECT * FROM Cap";  // Truy vấn tất cả các cấp hành chính
                DataTable dt = _dbHelper.ExecuteQuery(query);
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while fetching Cap data: {ex.Message}");
                return new DataTable(); // Trả về DataTable rỗng trong trường hợp lỗi
            }
        }

        // Lấy danh sách đơn vị hành chính theo cấp (cấp 1: huyện, cấp 2: xã)
        public DataTable GetDanhMucDonViHanhChinh(int capId)
        {
            try
            {
                string query = @"
                    SELECT D.ID, D.TenDVHC, C.TenCap, D.parent_id
                    FROM DonViHanhChinh D
                    JOIN Cap C ON D.ID_Cap = C.ID
                    WHERE D.ID_Cap = @CapId"; // Lọc theo cấp hành chính
                var parameters = new[] 
                {
                    new SqlParameter("@CapId", SqlDbType.Int) { Value = capId }
                };
                DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while fetching DonViHanhChinh data: {ex.Message}");
                return new DataTable(); // Trả về DataTable rỗng trong trường hợp lỗi
            }
        }

        // Lấy danh sách xã theo huyện (Lấy xã theo parent_id)
        public DataTable GetDanhMucXaByHuyen(int huyenId)
        {
            try
            {
                string query = @"
                    SELECT D.ID, D.TenDVHC, C.TenCap
                    FROM DonViHanhChinh D
                    JOIN Cap C ON D.ID_Cap = C.ID
                    WHERE D.parent_id = @HuyenId AND D.ID_Cap = 2"; // Cấp 2 là xã
                var parameters = new[] 
                {
                    new SqlParameter("@HuyenId", SqlDbType.Int) { Value = huyenId }
                };
                DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while fetching Xa data: {ex.Message}");
                return new DataTable(); // Trả về DataTable rỗng trong trường hợp lỗi
            }
        }

        // Tìm kiếm huyện và xã dựa trên từ khóa
        // Tìm kiếm huyện và xã dựa trên từ khóa gần đúng
        public DataTable SearchDonViHanhChinh(string keyword)
        {
            try
            {
                string query = @"
            SELECT D.ID, D.TenDVHC, C.TenCap, D.parent_id
            FROM DonViHanhChinh D
            JOIN Cap C ON D.ID_Cap = C.ID
            WHERE D.TenDVHC COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @Keyword

            UNION

            SELECT Parent.ID, Parent.TenDVHC, CParent.TenCap, Parent.parent_id
            FROM DonViHanhChinh Child
            JOIN DonViHanhChinh Parent ON Child.parent_id = Parent.ID
            JOIN Cap CParent ON Parent.ID_Cap = CParent.ID
            WHERE Child.TenDVHC COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @Keyword
            AND Parent.ID_Cap = 2"; // Lấy cả Huyện chứa Xã tìm kiếm

                var parameters = new[]
                {
            new SqlParameter("@Keyword", SqlDbType.NVarChar) { Value = $"%{keyword}%" }
        };

                DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while searching DonViHanhChinh: {ex.Message}");
                return new DataTable();
            }
        }



    }
}
   
