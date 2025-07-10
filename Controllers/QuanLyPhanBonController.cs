using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PlantManagement.Helpers;
using System.Collections;

namespace PlantManagement.Controllers
{
    public class QuanLyPhanBonController
    {
        private readonly DatabaseHelper _databaseHelper;

        public QuanLyPhanBonController()
        {
            _databaseHelper = new DatabaseHelper();
        }


        public DataTable GetAllPhanBon()
        {
            string query = @"
    SELECT 
        PhanBon.ID,
        PhanBon.TenPhanBon,
        PhanBon.ThongTin,
        CoSoSanXuat.TenCS AS TenCSSX,
        CoSoBuonBan.TenCS AS TenCSBB
    FROM 
        PhanBon
    LEFT JOIN 
        CoSoSanXuat ON PhanBon.ID_CSSX = CoSoSanXuat.ID
    LEFT JOIN 
        CoSoBuonBan ON PhanBon.ID_CSBB = CoSoBuonBan.ID;
    ";

            // Gọi hàm ExecuteQuery từ _databaseHelper để thực thi truy vấn SQL và trả về DataTable
            return _databaseHelper.ExecuteQuery(query);
        }


        private string _searchCriterion = "TenPhanBon"; // Mặc định tìm kiếm theo Tên phân bón

        // Phương thức thay đổi tiêu chí tìm kiếm
        public void SetSearchCriterionByTenPhanBon()
        {
            _searchCriterion = "TenPhanBon"; // Tìm kiếm theo tên phân bón
        }

        public void SetSearchCriterionByCoSoSanXuat()
        {
            _searchCriterion = "TenCSSX"; // Đặt tiêu chí tìm kiếm là alias "TenCSSX" từ truy vấn SQL
        }



        public void SetSearchCriterionByCoSoBuonBan()
        {
            _searchCriterion = "TenCSBB"; // Tìm kiếm theo tên cơ sở buôn bán
        }

        public string GetSearchCriterion()
        {
            return _searchCriterion;
        }

        public Visibility ToggleSearchOptionsVisibility(Visibility currentVisibility)
        {
            return currentVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        public void ResetSearch(Action reloadData)
        {
            try
            {
                // Reset các tiêu chí tìm kiếm (nếu có lưu trữ tiêu chí)
                // Xóa từ khóa tìm kiếm, hoặc đặt lại tiêu chí tìm kiếm về mặc định (nếu cần)

                // Gọi lại phương thức LoadPhanBonData từ View để tải dữ liệu
                reloadData.Invoke();
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra khi trở về bảng chính: " + ex.Message);
            }
        }


        public DataTable PerformSearch(string keyword, string searchCriterion)
        {
            try
            {
                // Danh sách các cột hợp lệ để tìm kiếm
                var validCriteria = new Dictionary<string, string>
        {
            { "TenPhanBon", "PhanBon.TenPhanBon" },
            { "TenCSSX", "CoSoSanXuat.TenCS" },
            { "TenCSBB", "CoSoBuonBan.TenCS" }
        };

                // Kiểm tra xem searchCriterion có hợp lệ không
                if (!validCriteria.ContainsKey(searchCriterion))
                {
                    throw new ArgumentException("Tiêu chí tìm kiếm không hợp lệ.");
                }

                // Xây dựng câu truy vấn SQL
                string query = $@"
        SELECT 
            PhanBon.ID,
            PhanBon.TenPhanBon,
            PhanBon.ThongTin,
            CoSoSanXuat.TenCS AS TenCSSX,
            CoSoBuonBan.TenCS AS TenCSBB
        FROM 
            PhanBon
        LEFT JOIN 
            CoSoSanXuat ON PhanBon.ID_CSSX = CoSoSanXuat.ID
        LEFT JOIN 
            CoSoBuonBan ON PhanBon.ID_CSBB = CoSoBuonBan.ID
        WHERE 
            {validCriteria[searchCriterion]} LIKE @Keyword";

                // Thiết lập tham số truy vấn với từ khóa tìm kiếm
                var parameters = new SqlParameter[] {
            new SqlParameter("@Keyword", $"%{keyword}%")
        };

                // Thực thi truy vấn và trả về DataTable
                DataTable dataTable = _databaseHelper.ExecuteQuery(query, parameters);
                return dataTable;
            }
            catch (ArgumentException ex)
            {
                // Xử lý lỗi nếu tiêu chí tìm kiếm không hợp lệ
                throw new Exception("Lỗi: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi chung
                throw new Exception("Có lỗi xảy ra khi tìm kiếm: " + ex.Message);
            }
        }


        public void SearchOnEnter(string keyword, string searchCriterion, Action performSearch)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                // Gọi phương thức thực hiện tìm kiếm trong View
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

        public void SaveRecord(string tenPhanBon, string thongTin, string coSoSanXuat, string coSoBuonBan, int editRecordID, Action loadCoSoData, Action cancelButtonClick)
        {
            try
            {
                // Kiểm tra và thêm cơ sở sản xuất
                string queryCSSX = "SELECT ID FROM CoSoSanXuat WHERE TenCS = @TenCS";
                SqlParameter paramCSSX = new SqlParameter("@TenCS", coSoSanXuat);
                var resultCSSX = _databaseHelper.ExecuteScalar(queryCSSX, new SqlParameter[] { paramCSSX });

                int idCSSX = resultCSSX == null ? InsertCoSoSanXuat(coSoSanXuat) : Convert.ToInt32(resultCSSX);

                // Kiểm tra và thêm cơ sở buôn bán
                string queryCSBB = "SELECT ID FROM CoSoBuonBan WHERE TenCS = @TenCS";
                SqlParameter paramCSBB = new SqlParameter("@TenCS", coSoBuonBan);
                var resultCSBB = _databaseHelper.ExecuteScalar(queryCSBB, new SqlParameter[] { paramCSBB });

                int idCSBB = resultCSBB == null ? InsertCoSoBuonBan(coSoBuonBan) : Convert.ToInt32(resultCSBB);

                if (editRecordID != -1)
                {
                    // Cập nhật dữ liệu
                    string updateQuery = @"
                UPDATE PhanBon 
                SET TenPhanBon = @TenPhanBon, ThongTin = @ThongTin, ID_CSSX = @ID_CSSX, ID_CSBB = @ID_CSBB
                WHERE ID = @ID";

                    SqlParameter[] updateParameters = {
                new SqlParameter("@TenPhanBon", tenPhanBon),
                new SqlParameter("@ThongTin", thongTin),
                new SqlParameter("@ID_CSSX", idCSSX),
                new SqlParameter("@ID_CSBB", idCSBB),
                new SqlParameter("@ID", editRecordID)
            };

                    _databaseHelper.ExecuteNonQuery(updateQuery, updateParameters);
                }
                else
                {
                    // Thêm mới dữ liệu
                    string insertQuery = @"
                INSERT INTO PhanBon (TenPhanBon, ThongTin, ID_CSSX, ID_CSBB) 
                VALUES (@TenPhanBon, @ThongTin, @ID_CSSX, @ID_CSBB)";

                    SqlParameter[] insertParameters = {
                new SqlParameter("@TenPhanBon", tenPhanBon),
                new SqlParameter("@ThongTin", thongTin),
                new SqlParameter("@ID_CSSX", idCSSX),
                new SqlParameter("@ID_CSBB", idCSBB)
            };

                    _databaseHelper.ExecuteNonQuery(insertQuery, insertParameters);
                }

                loadCoSoData.Invoke();
                cancelButtonClick.Invoke();
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra: " + ex.Message);
            }
        }

        // Hàm thêm mới cơ sở sản xuất
        private int InsertCoSoSanXuat(string tenCS)
        {
            string insertQuery = "INSERT INTO CoSoSanXuat (TenCS) VALUES (@TenCS); SELECT SCOPE_IDENTITY();";
            return Convert.ToInt32(_databaseHelper.ExecuteScalar(insertQuery, new SqlParameter[] { new SqlParameter("@TenCS", tenCS) }));
        }

        // Hàm thêm mới cơ sở buôn bán
        private int InsertCoSoBuonBan(string tenCS)
        {
            string insertQuery = "INSERT INTO CoSoBuonBan (TenCS) VALUES (@TenCS); SELECT SCOPE_IDENTITY();";
            return Convert.ToInt32(_databaseHelper.ExecuteScalar(insertQuery, new SqlParameter[] { new SqlParameter("@TenCS", tenCS) }));
        }

        public void CancelEditing(Action clearFormAction, Action showDataGridAction)
        {
            try
            {
                // Reset form (clear form fields)
                clearFormAction.Invoke();

                // Hiển thị DataGrid và ẩn form thêm mới
                showDataGridAction.Invoke();
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra khi hủy: " + ex.Message);
            }
        }



        public void DeleteRecord(int id, Action reloadData)
        {
            try
            {
                string deleteQuery = "DELETE FROM PhanBon WHERE ID = @ID";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@ID", id)
                };

                int rowsAffected = _databaseHelper.ExecuteNonQuery(deleteQuery, parameters);

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    reloadData.Invoke();
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


        public int EditRecord(int id, Action<string, string, string, string> showEditFormAction)
        {
            try
            {
                // Câu truy vấn SQL
                string query = @"
        SELECT 
            PhanBon.TenPhanBon, 
            PhanBon.ThongTin, 
            CoSoSanXuat.TenCS AS TenCoSoSanXuat, 
            CoSoBuonBan.TenCS AS TenCoSoBuonBan
        FROM 
            PhanBon
        LEFT JOIN 
            CoSoSanXuat ON PhanBon.ID_CSSX = CoSoSanXuat.ID
        LEFT JOIN 
            CoSoBuonBan ON PhanBon.ID_CSBB = CoSoBuonBan.ID
        WHERE 
            PhanBon.ID = @ID";

                // Thực thi câu truy vấn SQL
                SqlParameter[] parameters = { new SqlParameter("@ID", id) };
                DataTable dataTable = _databaseHelper.ExecuteQuery(query, parameters);

                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];

                    // Gọi phương thức hiển thị thông tin lên form chỉnh sửa
                    showEditFormAction.Invoke(
                        row["TenPhanBon"].ToString(),
                        row["ThongTin"].ToString(),
                        row["TenCoSoSanXuat"] != DBNull.Value ? row["TenCoSoSanXuat"].ToString() : "Chưa có",
                        row["TenCoSoBuonBan"] != DBNull.Value ? row["TenCoSoBuonBan"].ToString() : "Chưa có"
                    );

                    return id; // Trả về ID bản ghi
                }
                else
                {
                    throw new Exception("Không tìm thấy bản ghi.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra khi lấy dữ liệu chỉnh sửa: " + ex.Message);
            }
        }




    }
}