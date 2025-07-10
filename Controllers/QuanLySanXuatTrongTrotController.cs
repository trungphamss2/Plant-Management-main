using PlantManagement.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlantManagement.Controllers;
using System.Windows.Input;
using System.Windows;



namespace PlantManagement.Controllers
{
    public class QuanLySanXuatTrongTrotController
    {
        private readonly DatabaseHelper _databaseHelper;

        public QuanLySanXuatTrongTrotController()
        {
            _databaseHelper = new DatabaseHelper();
        }

        public DataTable GetAllSXTT()
        {
            string query = @" SELECT SXTT.ID,SXTT.CoSo, SXTT.CSAnToanVietGap, SVGHT.TenSVGH AS SinhVatGayHai, DVHC.TenDVHC AS ViTri, SXTT.SanPham FROM SXTT LEFT JOIN DonViHanhChinh DVHC ON SXTT.ID_DVHC = DVHC.ID LEFT JOIN SinhVatGayHai SVGHT ON SXTT.ID_SVGH = SVGHT.ID";
            return _databaseHelper.ExecuteQuery(query);
        }

        public Visibility ToggleSearchOptionsVisibility(Visibility currentVisibility)
        {
            return currentVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private string _searchCriterion = "SanPham";

        // Phương thức thay đổi tiêu chí tìm kiếm
        public void SetSearchCriterionByRegion()
        {
            _searchCriterion = "DVHC.TenDVHC"; // Tìm kiếm theo tên vùng trồng
        }

        public void SetSearchCriterionByPest()
        {
            _searchCriterion = "SXTT.CoSo"; // Tìm kiếm theo tên sinh vật gây hại
        }

        public void SetSearchCriterionByProduct()
        {
            _searchCriterion = "SXTT.SanPham"; // Tìm kiếm theo sản phẩm
        }

        // Phương thức lấy tiêu chí tìm kiếm hiện tại
        public string GetSearchCriterion()
        {
            return _searchCriterion;
        }

        public DataTable PerformSearch(string keyword, string searchCriterion)
        {
            try
            {
                string query = $@"
            SELECT 
                SXTT.ID, 
                SXTT.CoSo,
                SXTT.CSAnToanVietGap, 
                SVGHT.TenSVGH AS SinhVatGayHai, 
                DVHC.TenDVHC AS ViTri, 
                SXTT.SanPham 
            FROM 
                SXTT 
                LEFT JOIN DonViHanhChinh DVHC ON SXTT.ID_DVHC = DVHC.ID 
                LEFT JOIN SinhVatGayHai SVGHT ON SXTT.ID_SVGH = SVGHT.ID
            WHERE 
                {searchCriterion} LIKE @Keyword";

                var parameters = new SqlParameter[] {
                new SqlParameter("@Keyword", $"%{keyword}%")
            };

                DataTable dataTable = _databaseHelper.ExecuteQuery(query, parameters);
                // Trả về DataTable để View cập nhật
                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra: " + ex.Message);
            }
        }

        public void ResetSearch(Action reloadData)
        {
            try
            {
                // Xóa từ khóa tìm kiếm
                // Gọi lại phương thức LoadCoSoData từ View
                reloadData.Invoke();
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra khi trở về bảng chính: " + ex.Message);
            }
        }

        public void SearchOnEnter(string keyword, string searchCriterion, Action performSearch)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                // Gọi phương thức thực hiện tìm kiếm
                performSearch.Invoke();
            }
        }

        public void ShowAddNewForm(Action hideDataGrid, Action showAddNewForm)
        {
            // Ẩn DataGrid
            hideDataGrid.Invoke();

            // Hiển thị Form thêm mới
            showAddNewForm.Invoke();
        }

        public void ClearFormFields(Action clearFields)
        {
            // Gọi phương thức xóa các trường dữ liệu
            clearFields.Invoke();
        }

        // Phương thức lưu hoặc cập nhật bản ghi
        public void SaveRecord(bool vietGap, string sanPham, string coso, string vungTrong, string sinhVatGayHai, int editRecordID, Action loadCoSoData, Action cancelButtonClick)
        {
            try
            {
                // Kiểm tra và thêm vùng trồng nếu không tồn tại
                string queryDVHC = "SELECT ID FROM DonViHanhChinh WHERE TenDVHC = @VungTrong";
                SqlParameter paramDVHC = new SqlParameter("@VungTrong", vungTrong);
                var resultDVHC = _databaseHelper.ExecuteScalar(queryDVHC, new SqlParameter[] { paramDVHC });

                int idDVHC;
                if (resultDVHC == null)
                {
                    // Thêm vùng trồng mới vào cơ sở dữ liệu nếu không tồn tại
                    string insertQueryDVHC = "INSERT INTO DonViHanhChinh (TenDVHC) VALUES (@VungTrong)";
                    _databaseHelper.ExecuteNonQuery(insertQueryDVHC, new SqlParameter[] { paramDVHC });

                    // Lấy lại ID của vùng trồng vừa thêm
                    resultDVHC = _databaseHelper.ExecuteScalar(queryDVHC, new SqlParameter[] { paramDVHC });
                }

                idDVHC = Convert.ToInt32(resultDVHC);

                // Kiểm tra và thêm sinh vật gây hại nếu không tồn tại
                string querySVGH = "SELECT ID FROM SinhVatGayHai WHERE TenSVGH = @SinhVatGayHai";
                SqlParameter paramSVGH = new SqlParameter("@SinhVatGayHai", sinhVatGayHai);
                var resultSVGH = _databaseHelper.ExecuteScalar(querySVGH, new SqlParameter[] { paramSVGH });

                int idSVGH;
                if (resultSVGH == null)
                {
                    // Thêm sinh vật gây hại mới vào cơ sở dữ liệu nếu không tồn tại
                    string insertSVGHQuery = "INSERT INTO SinhVatGayHai (TenSVGH) VALUES (@SinhVatGayHai)";
                    _databaseHelper.ExecuteNonQuery(insertSVGHQuery, new SqlParameter[] { paramSVGH });

                    // Lấy lại ID của sinh vật gây hại vừa thêm
                    resultSVGH = _databaseHelper.ExecuteScalar(querySVGH, new SqlParameter[] { paramSVGH });
                }

                idSVGH = Convert.ToInt32(resultSVGH);

                // Kiểm tra nếu đang chỉnh sửa, thực hiện cập nhật
                if (editRecordID != -1)
                {
                    // Chỉnh sửa dữ liệu
                    string updateQuery = @"
                UPDATE SXTT 
                SET CSAnToanVietGap = @VietGap, SanPham = @SanPham, CoSo = @CoSo, ID_SVGH = @ID_SVGH, ID_DVHC = @ID_DVHC
                WHERE ID = @ID";

                    SqlParameter[] updateParameters = new SqlParameter[]
                    {
                    new SqlParameter("@VietGap", vietGap),
                    new SqlParameter("@CoSo", coso),
                    new SqlParameter("@SanPham", sanPham),
                    new SqlParameter("@ID_SVGH", idSVGH),
                    new SqlParameter("@ID_DVHC", idDVHC),
                    new SqlParameter("@ID", editRecordID)  // Chỉ định ID của bản ghi cần chỉnh sửa
                    };

                    int rowsAffected = _databaseHelper.ExecuteNonQuery(updateQuery, updateParameters);

                    if (rowsAffected > 0)
                    {
                        loadCoSoData.Invoke(); // Tải lại dữ liệu
                        cancelButtonClick.Invoke(); // Quay lại bảng
                    }
                    else
                    {
                        throw new Exception("Không thể cập nhật. Vui lòng kiểm tra dữ liệu!");
                    }
                }
                else
                {
                    // Thêm mới dữ liệu
                    string insertQuery = @"
                INSERT INTO SXTT (CSAnToanVietGap, CoSo, SanPham, ID_SVGH, ID_DVHC)
                VALUES (@VietGap, @CoSo, @SanPham, @ID_SVGH, @ID_DVHC)";

                    SqlParameter[] insertParameters = new SqlParameter[]
                    {
                    new SqlParameter("@VietGap", vietGap),
                    new SqlParameter("@CoSo", coso),
                    new SqlParameter("@SanPham", sanPham),
                    new SqlParameter("@ID_SVGH", idSVGH),
                    new SqlParameter("@ID_DVHC", idDVHC)
                    };

                    int rowsAffected = _databaseHelper.ExecuteNonQuery(insertQuery, insertParameters);

                    if (rowsAffected > 0)
                    {
                        loadCoSoData.Invoke(); // Tải lại dữ liệu
                        cancelButtonClick.Invoke(); // Quay lại bảng
                    }
                    else
                    {
                        throw new Exception("Không thể thêm mới. Vui lòng kiểm tra dữ liệu!");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra: " + ex.Message);
            }
        }

        public void CancelEditing(Action clearFormAction, Action showDataGridAction)
        {
            try
            {
                // Hiển thị DataGrid và ẩn form thêm mới
                showDataGridAction.Invoke();

                // Reset form (clear form fields)
                clearFormAction.Invoke();
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra khi hủy: " + ex.Message);
            }
        }

        public void DeleteRecord(int id, Action loadDataAction)
        {
            try
            {
                // Câu lệnh SQL để xóa bản ghi
                string deleteQuery = "DELETE FROM SXTT WHERE ID = @ID";
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@ID", id)
                };

                int rowsAffected = _databaseHelper.ExecuteNonQuery(deleteQuery, parameters);

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    loadDataAction.Invoke(); // Tải lại dữ liệu
                }
                else
                {
                    MessageBox.Show("Không thể xóa bản ghi!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Phương thức xử lý chỉnh sửa thông tin bản ghi
        public int EditRecord(int id, Action<string, string, string, string, bool> showEditFormAction)
        {
            try
            {
                // Câu lệnh SQL để lấy dữ liệu chỉnh sửa
                string query = @"
            SELECT SXTT.SanPham, SXTT.CoSo, SVGHT.TenSVGH, DVHC.TenDVHC, SXTT.CSAnToanVietGap 
            FROM SXTT 
            JOIN SinhVatGayHai SVGHT ON SXTT.ID_SVGH = SVGHT.ID 
            JOIN DonViHanhChinh DVHC ON SXTT.ID_DVHC = DVHC.ID 
            WHERE SXTT.ID = @ID";

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@ID", id) };

                // Lấy thông tin bản ghi cần chỉnh sửa
                DataTable dataTable = _databaseHelper.ExecuteQuery(query, parameters);

                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];

                    // Gọi hàm showEditFormAction để hiển thị thông tin lên form
                    showEditFormAction.Invoke(
                        row["SanPham"].ToString(),
                        row["CoSo"].ToString(),
                        row["TenSVGH"].ToString(),
                        row["TenDVHC"].ToString(),
                        Convert.ToBoolean(row["CSAnToanVietGap"])
                    );

                    return id; // Trả về ID bản ghi để lưu lại
                }
                else
                {
                    throw new Exception("Không tìm thấy bản ghi để chỉnh sửa.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra khi lấy dữ liệu chỉnh sửa: " + ex.Message);
            }
        }

    }
}