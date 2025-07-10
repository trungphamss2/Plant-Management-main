using PlantManagement.Helpers;
using PlantManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantManagement.Controllers
{
    public class ThuocBaoVeThucVatController
    {
        private readonly DatabaseHelper _dbHelper;
        public ThuocBaoVeThucVatController()
        {
            _dbHelper = new DatabaseHelper();
        }

        public DataTable GetAllThuocBVTVs(string searchString = null)
        {
            string query = @"
                SELECT 
                    tbtv.ID AS ID,
                    tbtv.TenThuoc AS TenThuocBVTV,
                    tbtv.ThongTin AS ThongTinThuoc,
                    cssx.TenCS AS TenCoSoSanXuat,
                    dvcssx.TenDVHC AS DiaChiCoSoSanXuat,
                    csbb.TenCS AS TenCoSoBuonBan,
                    dvcsbb.TenDVHC AS DiaChiCoSoBuonBan
                FROM 
                    ThuocBVTV tbtv
                LEFT JOIN 
                    CoSoSanXuat cssx ON tbtv.ID_CSSX = cssx.ID
                LEFT JOIN 
                    CoSoBuonBan csbb ON tbtv.ID_CSBB = csbb.ID
                LEFT JOIN 
                    DonViHanhChinh dvcssx ON cssx.ID_DVHC = dvcssx.ID
                LEFT JOIN 
                    DonViHanhChinh dvcsbb ON csbb.ID_DVHC = dvcsbb.ID; ";
            // Khởi tạo parameters
            var parameters = string.IsNullOrEmpty(searchString)
                ? new SqlParameter[]
                  {
              new SqlParameter("@SearchString", DBNull.Value) // Nếu không có giá trị, truyền NULL
                  }
                : new SqlParameter[]
                  {
              new SqlParameter("@SearchString", SqlDbType.NVarChar) { Value = "%" + searchString + "%" }
                  };

            // Gọi ExecuteQuery
            return _dbHelper.ExecuteQuery(query, parameters);
        }
        public DataTable GetAllCSSXs(string searchString = null)
        {
            string query = @"
                SELECT 
	                cssx.ID AS ID,
	                cssx.TenCS AS TenCoSoSanXuat,
	                dvcssx.TenDVHC AS DiaChiCoSoSanXuat
                FROM 
	                CoSoSanXuat cssx 
                LEFT JOIN 
	                DonViHanhChinh dvcssx ON cssx.ID_DVHC = dvcssx.ID; ";
            var parameters = string.IsNullOrEmpty(searchString)
                ? new SqlParameter[]
                  {
              new SqlParameter("@SearchString", DBNull.Value) // Nếu không có giá trị, truyền NULL
                  }
                : new SqlParameter[]
                  {
              new SqlParameter("@SearchString", SqlDbType.NVarChar) { Value = "%" + searchString + "%" }
                  };
            // Gọi ExecuteQuery
            return _dbHelper.ExecuteQuery(query, parameters);
        }
        public DataTable GetAllCSBBs(string searchString = null)
        {
            string query = @"
                SELECT 
	                csbb.ID,
	                csbb.TenCS AS TenCoSoBuonBan,
	                dvcsbb.TenDVHC AS DiaChiCoSoBuonBan
                FROM 
	                CoSoBuonBan csbb
                LEFT JOIN 
	                DonViHanhChinh dvcsbb ON csbb.ID_DVHC = dvcsbb.ID;";
            var parameters = string.IsNullOrEmpty(searchString)
            ? new SqlParameter[]
                  {
              new SqlParameter("@SearchString", DBNull.Value) // Nếu không có giá trị, truyền NULL
                  }
                : new SqlParameter[]
                  {
              new SqlParameter("@SearchString", SqlDbType.NVarChar) { Value = "%" + searchString + "%" }
                  };

            // Gọi ExecuteQuery
            return _dbHelper.ExecuteQuery(query, parameters);
        }
        // Xóa thuốc bảo vệ thực vật 
        public int DeleteThuocBVTV(int id)
        {
            string query = @"
                DELETE FROM ThuocBVTV WHERE ID = @ID";

            // Tạo parameter
            var parameters = new[]
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = id }
            };

            return _dbHelper.ExecuteNonQuery(query, parameters);
        }
        public bool UpdateThuocBVTV(int id, string tenThuoc, string thongTin, string tenCSSX, string diaChiCSSX, string tenCSBB, string diaChiCSBB)
        {
            string query = @"
        UPDATE ThuocBVTV
        SET 
            TenThuoc = @TenThuoc,
            ThongTin = @ThongTin,
            ID_CSSX = (
                SELECT TOP 1 cssx.ID 
                FROM CoSoSanXuat cssx
                JOIN DonViHanhChinh dv ON cssx.ID_DVHC = dv.ID
                WHERE cssx.TenCS = @TenCSSX AND dv.TenDVHC = @DiaChiCSSX
            ),
            ID_CSBB = (
                SELECT TOP 1 csbb.ID
                FROM CoSoBuonBan csbb
                JOIN DonViHanhChinh dv ON csbb.ID_DVHC = dv.ID
                WHERE csbb.TenCS = @TenCSBB AND dv.TenDVHC = @DiaChiCSBB
            )
        WHERE 
            ID = @ID";

            // Tạo danh sách tham số
            var parameters = new SqlParameter[]
            {
        new SqlParameter("@ID", SqlDbType.Int) { Value = id },
        new SqlParameter("@TenThuoc", SqlDbType.NVarChar) { Value = tenThuoc },
        new SqlParameter("@ThongTin", SqlDbType.NVarChar) { Value = thongTin },
        new SqlParameter("@TenCSSX", SqlDbType.NVarChar) { Value = tenCSSX },
        new SqlParameter("@DiaChiCSSX", SqlDbType.NVarChar) { Value = diaChiCSSX },
        new SqlParameter("@TenCSBB", SqlDbType.NVarChar) { Value = tenCSBB },
        new SqlParameter("@DiaChiCSBB", SqlDbType.NVarChar) { Value = diaChiCSBB }
            };

            // Thực thi lệnh
            int rowsAffected = _dbHelper.ExecuteNonQuery(query, parameters);

            // Trả về true nếu có dòng bị ảnh hưởng
            return rowsAffected > 0;
        }

        public bool AddThuocBVTV(string tenThuoc, string thongTin, string tenCSSX, string diaChiCSSX, string tenCSBB, string diaChiCSBB)
        {
            try
            {
                // Xử lý địa chỉ cơ sở sản xuất
                int idDVHCCSSX = GetOrAddDonViHanhChinh(diaChiCSSX);
                if (idDVHCCSSX == -1) throw new Exception("Không thể xử lý địa chỉ cơ sở sản xuất.");

                // Thêm cơ sở sản xuất nếu chưa tồn tại
                string insertCSSXQuery = @"
            IF NOT EXISTS (SELECT 1 FROM CoSoSanXuat WHERE TenCS = @TenCSSX AND ID_DVHC = @ID_DVHCCSSX)
            BEGIN
                INSERT INTO CoSoSanXuat (TenCS, ID_DVHC)
                VALUES (@TenCSSX, @ID_DVHCCSSX)
            END";

                var cssxParams = new[]
                {
                    new SqlParameter("@TenCSSX", tenCSSX),
                    new SqlParameter("@ID_DVHCCSSX", idDVHCCSSX)
                };
                _dbHelper.ExecuteNonQuery(insertCSSXQuery, cssxParams);

                // Xử lý địa chỉ cơ sở buôn bán
                int idDVHCCSBB = GetOrAddDonViHanhChinh(diaChiCSBB);
                if (idDVHCCSBB == -1) throw new Exception("Không thể xử lý địa chỉ cơ sở buôn bán.");

                // Thêm cơ sở buôn bán nếu chưa tồn tại
                string insertCSBBQuery = @"
            IF NOT EXISTS (SELECT 1 FROM CoSoBuonBan WHERE TenCS = @TenCSBB AND ID_DVHC = @ID_DVHCCSBB)
            BEGIN
                INSERT INTO CoSoBuonBan (TenCS, ID_DVHC)
                VALUES (@TenCSBB, @ID_DVHCCSBB)
            END";

                var csbbParams = new[]
                {
            new SqlParameter("@TenCSBB", tenCSBB),
            new SqlParameter("@ID_DVHCCSBB", idDVHCCSBB)
        };
                _dbHelper.ExecuteNonQuery(insertCSBBQuery, csbbParams);

                // Thêm thuốc bảo vệ thực vật
                string insertThuocBVTVQuery = @"
            INSERT INTO ThuocBVTV (TenThuoc, ThongTin, ID_CSSX, ID_CSBB)
            VALUES (@TenThuoc, @ThongTin, 
                    (SELECT TOP 1 ID FROM CoSoSanXuat WHERE TenCS = @TenCSSX AND ID_DVHC = @ID_DVHCCSSX),
                    (SELECT TOP 1 ID FROM CoSoBuonBan WHERE TenCS = @TenCSBB AND ID_DVHC = @ID_DVHCCSBB))";

                var thuocParams = new[]
                {
                    new SqlParameter("@TenThuoc", tenThuoc),
                    new SqlParameter("@ThongTin", thongTin),
                    new SqlParameter("@TenCSSX", tenCSSX),
                    new SqlParameter("@ID_DVHCCSSX", idDVHCCSSX),
                    new SqlParameter("@TenCSBB", tenCSBB),
                    new SqlParameter("@ID_DVHCCSBB", idDVHCCSBB)
                };
                _dbHelper.ExecuteNonQuery(insertThuocBVTVQuery, thuocParams);

                return true; // Thành công
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false; // Thất bại
            }
        }

        // Hàm xử lý địa chỉ thành đơn vị hành chính
        private int GetOrAddDonViHanhChinh(string diaChi)
        {
            // Tách địa chỉ thành Tỉnh, Huyện, Xã (giả sử cách nhau bởi dấu phẩy)
            var parts = diaChi.Split(',').Select(p => p.Trim()).ToArray();
            if (parts.Length < 3) return -1; // Địa chỉ không hợp lệ

            string xa = parts[0];
            string huyen = parts[1];
            string tinh = parts[2];

            // Thêm Tỉnh
            string insertTinhQuery = @"
        IF NOT EXISTS (SELECT 1 FROM DonViHanhChinh WHERE TenDVHC = @TenTinh AND parent_id IS NULL)
        BEGIN
            INSERT INTO DonViHanhChinh (TenDVHC, ID_Cap)
            VALUES (@TenTinh, (SELECT ID FROM Cap WHERE TenCap = 'Tỉnh'))
        END";
            var tinhParams = new[] { new SqlParameter("@TenTinh", tinh) };
            _dbHelper.ExecuteNonQuery(insertTinhQuery, tinhParams);

            // Thêm Huyện
            string insertHuyenQuery = @"
        IF NOT EXISTS (SELECT 1 FROM DonViHanhChinh WHERE TenDVHC = @TenHuyen AND parent_id = 
                       (SELECT ID FROM DonViHanhChinh WHERE TenDVHC = @TenTinh AND parent_id IS NULL))
        BEGIN
            INSERT INTO DonViHanhChinh (TenDVHC, ID_Cap, parent_id)
            VALUES (@TenHuyen, (SELECT ID FROM Cap WHERE TenCap = 'Huyện'), 
                    (SELECT ID FROM DonViHanhChinh WHERE TenDVHC = @TenTinh AND parent_id IS NULL))
        END";
            var huyenParams = new[]
            {
        new SqlParameter("@TenHuyen", huyen),
        new SqlParameter("@TenTinh", tinh)
    };
            _dbHelper.ExecuteNonQuery(insertHuyenQuery, huyenParams);

            // Thêm Xã
            string insertXaQuery = @"
        IF NOT EXISTS (SELECT 1 FROM DonViHanhChinh WHERE TenDVHC = @TenXa AND parent_id = 
                       (SELECT ID FROM DonViHanhChinh WHERE TenDVHC = @TenHuyen AND parent_id = 
                       (SELECT ID FROM DonViHanhChinh WHERE TenDVHC = @TenTinh AND parent_id IS NULL)))
        BEGIN
            INSERT INTO DonViHanhChinh (TenDVHC, ID_Cap, parent_id)
            VALUES (@TenXa, (SELECT ID FROM Cap WHERE TenCap = 'Xã'), 
                    (SELECT ID FROM DonViHanhChinh WHERE TenDVHC = @TenHuyen AND parent_id = 
                    (SELECT ID FROM DonViHanhChinh WHERE TenDVHC = @TenTinh AND parent_id IS NULL)))
        END";
            var xaParams = new[]
            {
        new SqlParameter("@TenXa", xa),
        new SqlParameter("@TenHuyen", huyen),
        new SqlParameter("@TenTinh", tinh)
    };
            _dbHelper.ExecuteNonQuery(insertXaQuery, xaParams);

            // Lấy ID của Xã
            string getXaIdQuery = @"
        SELECT ID FROM DonViHanhChinh 
        WHERE TenDVHC = @TenXa AND parent_id = 
              (SELECT ID FROM DonViHanhChinh WHERE TenDVHC = @TenHuyen AND parent_id = 
              (SELECT ID FROM DonViHanhChinh WHERE TenDVHC = @TenTinh AND parent_id IS NULL))";
            var xaIdParams = new[]
            {
        new SqlParameter("@TenXa", xa),
        new SqlParameter("@TenHuyen", huyen),
        new SqlParameter("@TenTinh", tinh)
    };
            object result = _dbHelper.ExecuteScalar(getXaIdQuery, xaIdParams);

            return result != null ? Convert.ToInt32(result) : -1;
        }

    }

}