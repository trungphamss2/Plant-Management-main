using PlantManagement.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PlantManagement.Controllers
{
    public class GiongCayTrongController
    {
        private readonly DatabaseHelper _dbHelper;

        public GiongCayTrongController()
        {
            _dbHelper = new DatabaseHelper(); // Khởi tạo DatabaseHelper
        }
        // Lấy danh sách tất cả giống cây trồng 
        public DataTable GetAllGiongCayTrongs(string searchString = null)
        {
            string query = @"
                SELECT 
                    GCT.ID,
                    GCT.TenGiong,
                    GCT.MoTa,
                    GCT.LoaiCay,
                    STRING_AGG(DVHC.TenDVHC, ', ') AS NoiLuuHanh,
                    VCDD.TenVuonCay,
                    DVHCVuonCay.TenDVHC AS DiaChiVuonCay
                FROM GiongCayTrong AS GCT
                LEFT JOIN GiongCay_LuuHanh AS GCLH ON GCT.ID = GCLH.ID_GiongCay
                LEFT JOIN DonViHanhChinh AS DVHC ON GCLH.ID_DVHC = DVHC.ID
                LEFT JOIN VuonCayDauDong AS VCDD ON GCT.ID = VCDD.ID_GiongCayTrong
                LEFT JOIN DonViHanhChinh AS DVHCVuonCay ON VCDD.ID_DVHC = DVHCVuonCay.ID
                GROUP BY 
                    GCT.ID, 
                    GCT.TenGiong, 
                    GCT.MoTa, 
                    GCT.LoaiCay, 
                    VCDD.TenVuonCay, 
                    DVHCVuonCay.TenDVHC ";
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


        //thêm giống cây trồng mới
        public bool AddGiongCayTrong(
    string tenGiong,
    string moTa,
    string loaiCay,
    List<string> noiLuuHanh,
    List<string> tenVuonCay,
    List<string> diaChiVuonCay)
        {
            try
            {
                // 1. Thêm giống cây trồng
                string queryGiongCay = @"
                    INSERT INTO GiongCayTrong (TenGiong, MoTa, LoaiCay) 
                    VALUES (@TenGiong, @MoTa, @LoaiCay);
                    SELECT SCOPE_IDENTITY();";
                var parametersGiongCay = new[]
                {
                    new SqlParameter("@TenGiong", SqlDbType.NVarChar) { Value = tenGiong },
                    new SqlParameter("@MoTa", SqlDbType.NVarChar) { Value = moTa },
                    new SqlParameter("@LoaiCay", SqlDbType.NVarChar) { Value = loaiCay }
                };
                int idGiongCayTrong = Convert.ToInt32(_dbHelper.ExecuteScalar(queryGiongCay, parametersGiongCay));

                // 2. Thêm nơi lưu hành
                if (noiLuuHanh != null && noiLuuHanh.Count > 0)
                {
                    foreach (var diaChi in noiLuuHanh)
                    {
                        int idDVHC = GetOrAddDonViHanhChinh(diaChi);
                        string queryLuuHanh = @"
                    INSERT INTO GiongCay_LuuHanh (ID_GiongCay, ID_DVHC)
                    VALUES (@ID_GiongCay, @ID_DVHC)";
                        var parametersLuuHanh = new[]
                        {
                            new SqlParameter("@ID_GiongCay", SqlDbType.Int) { Value = idGiongCayTrong },
                            new SqlParameter("@ID_DVHC", SqlDbType.Int) { Value = idDVHC }
                        };
                        _dbHelper.ExecuteNonQuery(queryLuuHanh, parametersLuuHanh);
                    }
                }

                // 3. Thêm thông tin vườn cây đầu dòng
                if (tenVuonCay != null && diaChiVuonCay != null && tenVuonCay.Count > 0)
                {
                    for (int i = 0; i < tenVuonCay.Count; i++)
                    {
                        int idDVHC = GetOrAddDonViHanhChinh(diaChiVuonCay[i]);
                        string queryVuonCay = @"
                            INSERT INTO VuonCayDauDong (ID_GiongCayTrong, TenVuonCay, ID_DVHC)
                            VALUES (@ID_GiongCayTrong, @TenVuonCay, @ID_DVHC)";
                        var parametersVuonCay = new[]
                        {
                            new SqlParameter("@ID_GiongCayTrong", SqlDbType.Int) { Value = idGiongCayTrong },
                            new SqlParameter("@TenVuonCay", SqlDbType.NVarChar) { Value = tenVuonCay[i] },
                            new SqlParameter("@ID_DVHC", SqlDbType.Int) { Value = idDVHC }
                        };
                        _dbHelper.ExecuteNonQuery(queryVuonCay, parametersVuonCay);
                    }
                }

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
        // Xóa một giống cây trồng theo ID
        public int DeleteGiongCayTrong(int id)
        {
            string query = "DELETE FROM GiongCayTrong WHERE ID = @ID";

            var parameters = new[]
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = id }
            };

            return _dbHelper.ExecuteNonQuery(query, parameters);
        }


        // Cập nhật thông tin giống cây trồng

        public bool UpdateGiongCayTrong(int idGiongCay, string tenGiong, string moTa, string loaiCay,
    List<string> dsNoiLuuHanh, List<string> dsVuonCay, List<string> dsDiaChiVuonCay)
        {
            try
            {
                // 1. Cập nhật thông tin giống cây trồng trong bảng GiongCayTrong
                string queryUpdateGiongCayTrong = @"
                UPDATE GiongCayTrong
                SET TenGiong = @TenGiong,
                    MoTa = @MoTa,
                    LoaiCay = @LoaiCay
                WHERE ID = @ID";

                var parametersUpdateGiongCayTrong = new[]
                {
                    new SqlParameter("@ID", SqlDbType.Int) { Value = idGiongCay },
                    new SqlParameter("@TenGiong", SqlDbType.NVarChar) { Value = tenGiong },
                    new SqlParameter("@MoTa", SqlDbType.NVarChar) { Value = moTa },
                    new SqlParameter("@LoaiCay", SqlDbType.NVarChar) { Value = loaiCay }
                };

                _dbHelper.ExecuteNonQuery(queryUpdateGiongCayTrong, parametersUpdateGiongCayTrong);

                // 2. Cập nhật nơi lưu hành
                if (dsNoiLuuHanh != null && dsNoiLuuHanh.Count > 0)
                {
                    foreach (var diaChi in dsNoiLuuHanh)
                    {
                        // Tách địa chỉ thành xã, huyện, tỉnh
                        int idDVHC = GetOrAddDonViHanhChinh(diaChi);
                        if (idDVHC == -1)
                        {
                            throw new Exception($"Không thể xử lý địa chỉ: {diaChi}");
                        }

                        // Thêm thông tin nơi lưu hành
                        string queryInsertLuuHanh = @"
                            IF NOT EXISTS (SELECT 1 FROM GiongCay_LuuHanh WHERE ID_GiongCay = @ID_GiongCay AND ID_DVHC = @ID_DVHC)
                            BEGIN
                                INSERT INTO GiongCay_LuuHanh (ID_GiongCay, ID_DVHC)
                                VALUES (@ID_GiongCay, @ID_DVHC)
                            END";
                        var parametersInsertLuuHanh = new[]
                        {
                            new SqlParameter("@ID_GiongCay", SqlDbType.Int) { Value = idGiongCay },
                            new SqlParameter("@ID_DVHC", SqlDbType.Int) { Value = idDVHC }
                        };
                        _dbHelper.ExecuteNonQuery(queryInsertLuuHanh, parametersInsertLuuHanh);
                    }
                }
                // 3. Cập nhật vườn cây đầu dòng (dùng danh sách tên và địa chỉ vườn cây)
                if (dsVuonCay != null && dsVuonCay.Count > 0 && dsDiaChiVuonCay != null && dsDiaChiVuonCay.Count > 0)
                {
                    for (int i = 0; i < dsVuonCay.Count; i++)
                    {
                        string tenVuonCay = dsVuonCay[i];
                        string diaChiVuonCay = dsDiaChiVuonCay[i];

                        // Lấy ID của đơn vị hành chính từ địa chỉ vườn cây
                        int idDVHCVuonCay = GetOrAddDonViHanhChinh(diaChiVuonCay);
                        if (idDVHCVuonCay == -1)
                        {
                            throw new Exception($"Không thể xử lý địa chỉ vườn cây: {diaChiVuonCay}");
                        }

                        // Kiểm tra và thêm thông tin vườn cây nếu chưa tồn tại
                        string queryUpdateVuonCay = @"
                            IF NOT EXISTS (SELECT 1 FROM VuonCayDauDong 
                                           WHERE TenVuonCay = @TenVuonCay AND ID_DVHC = @ID_DVHC)
                            BEGIN
                                INSERT INTO VuonCayDauDong (ID_GiongCayTrong, TenVuonCay, ID_DVHC)
                                VALUES (@ID_GiongCayTrong, @TenVuonCay, @ID_DVHC)
                            END";

                        var parametersUpdateVuonCay = new[]
                        {
                            new SqlParameter("@ID_GiongCayTrong", SqlDbType.Int) { Value = idGiongCay },
                            new SqlParameter("@TenVuonCay", SqlDbType.NVarChar) { Value = tenVuonCay },
                            new SqlParameter("@ID_DVHC", SqlDbType.Int) { Value = idDVHCVuonCay }
                        };

                        _dbHelper.ExecuteNonQuery(queryUpdateVuonCay, parametersUpdateVuonCay);
                    }
                }


                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật giống cây trồng: {ex.Message}");
                return false;
            }
        }



        // Lấy danh sách Vườn cây đầu dòng
        public DataTable GetAllVuonCayDauDong(string searchString = null)
        {
            // Câu lệnh SQL để lấy thông tin vườn cây đầu dòng, tên cây đầu dòng và đơn vị hành chính
            string query = @"
                SELECT g.ID, v.TenVuonCay, g.TenGiong , d.TenDVHC
                FROM VuonCayDauDong v
                LEFT JOIN GiongCayTrong g ON v.ID_GiongCayTrong = g.ID
                LEFT JOIN DonViHanhChinh d ON v.ID_DVHC = d.ID
                WHERE g.LoaiCay = N'Cây đầu dòng'
                ;";  // Đảm bảo chỉ lấy cây đầu dòng

            // Nếu có từ khóa tìm kiếm, thêm điều kiện vào câu truy vấn
            if (!string.IsNullOrEmpty(searchString))
            {
                query += " AND v.TenVuonCay LIKE @SearchString";
            }

            // Các tham số cho câu lệnh SQL
            var parameters = string.IsNullOrEmpty(searchString)
                ? null
                : new[]
                {
            new SqlParameter("@SearchString", SqlDbType.NVarChar) { Value = "%" + searchString + "%" }
                };

            // Thực thi câu lệnh SQL và trả về kết quả dưới dạng DataTable
            return _dbHelper.ExecuteQuery(query, parameters);
        }
        public DataTable GetAllCayDauDong(string searchString = null)
        {
            string query = @"
            SELECT 
                GCT.ID,
                GCT.TenGiong ,
                GCT.MoTa,
                STRING_AGG(DVHC.TenDVHC, ', ') AS NoiLuuHanh
            FROM GiongCayTrong AS GCT
            LEFT JOIN GiongCay_LuuHanh AS GCLH ON GCT.ID = GCLH.ID_GiongCay
            LEFT JOIN DonViHanhChinh AS DVHC ON GCLH.ID_DVHC = DVHC.ID
            WHERE GCT.LoaiCay = N'Cây đầu dòng'
            GROUP BY 
                GCT.ID, 
                GCT.TenGiong, 
                GCT.MoTa;";

            // Nếu có searchString, thêm điều kiện tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                query += " AND c.TenGiong LIKE @SearchString";
            }

            // Tạo tham số để tránh SQL Injection
            var parameters = string.IsNullOrEmpty(searchString)
                ? null
                : new[]
                {
            new SqlParameter("@SearchString", SqlDbType.NVarChar) { Value = "%" + searchString + "%" }
                };

            // Thực thi câu truy vấn và trả về kết quả dưới dạng DataTable
            return _dbHelper.ExecuteQuery(query, parameters);
        }

        public DataTable GetAllGiongCayTrongChinh(string searchString = null)
        {
            string query = @"
            SELECT 
                c.ID,
                c.TenGiong, 
                c.MoTa, 
                STRING_AGG(d.TenDVHC, ', ') AS NoiLuuHanh
            FROM 
                GiongCayTrong c
            INNER JOIN 
                GiongCay_LuuHanh gch ON c.ID = gch.ID_GiongCay
            INNER JOIN 
                DonViHanhChinh d ON gch.ID_DVHC = d.ID
            WHERE 
                c.LoaiCay = N'Giống chính'
            GROUP BY 
                c.ID, 
                c.TenGiong, 
                c.MoTa;";  // Điều kiện lọc giống cây đầu dòng

            // Nếu có searchString, thêm điều kiện tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                query += " AND c.TenGiong LIKE @SearchString";
            }

            // Tạo tham số để tránh SQL Injection
            var parameters = string.IsNullOrEmpty(searchString)
                ? null
                : new[]
                {
            new SqlParameter("@SearchString", SqlDbType.NVarChar) { Value = "%" + searchString + "%" }
                };

            // Thực thi câu truy vấn và trả về kết quả dưới dạng DataTable
            return _dbHelper.ExecuteQuery(query, parameters);
        }
    }
}