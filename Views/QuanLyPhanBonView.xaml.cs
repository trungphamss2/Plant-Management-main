using PlantManagement.Controllers;
using PlantManagement.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlantManagement.Views
{
    /// <summary>
    /// Interaction logic for QuanLySanXuatTrongTrotView.xaml
    /// </summary>
    public partial class QuanLyPhanBonView : UserControl
    {
        private DatabaseHelper _databaseHelper;
        private QuanLyPhanBonController _controller;

        public QuanLyPhanBonView()
        {
            InitializeComponent();
            _controller = new QuanLyPhanBonController();
            LoadPhanBonData(); // Thay vì LoadCoSoData, gọi hàm tải dữ liệu Phân Bón
        }

        private void LoadPhanBonData()
        {
            try
            {
                var dataTable = _controller.GetAllPhanBon();
                DataGridPhanBon.ItemsSource = dataTable.DefaultView; // DataGrid cho bảng Phân Bón
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Hiển thị/ẩn tùy chọn tìm kiếm
            SearchOptions.Visibility = _controller.ToggleSearchOptionsVisibility(SearchOptions.Visibility);
        }

        private void SearchByTenPhanBon_Click(object sender, RoutedEventArgs e)
        {
            _controller.SetSearchCriterionByTenPhanBon();
            SearchOptions.Visibility = Visibility.Collapsed;
        }

        private void SearchByCoSoSanXuat_Click(object sender, RoutedEventArgs e)
        {
            _controller.SetSearchCriterionByCoSoSanXuat();
            SearchOptions.Visibility = Visibility.Collapsed;
        }

        private void SearchByCoSoBuonBan_Click(object sender, RoutedEventArgs e)
        {
            _controller.SetSearchCriterionByCoSoBuonBan();
            SearchOptions.Visibility = Visibility.Collapsed;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Gọi phương thức từ Controller để reset tìm kiếm
                _controller.ResetSearch(LoadPhanBonData);

                // Ẩn các tiêu chí tìm kiếm (nếu cần)
                SearchOptions.Visibility = Visibility.Collapsed;
             
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi trở về bảng chính: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void PerformSearch()
        {
            try
            {
                string keyword = SearchTextBox.Text.Trim();

                // Gọi phương thức từ Controller để thực hiện tìm kiếm
                DataTable dataTable = _controller.PerformSearch(keyword, _controller.GetSearchCriterion());

                // Cập nhật DataGrid với kết quả tìm kiếm
                DataGridPhanBon.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // Khi nhấn phím Enter
            {
                string keyword = SearchTextBox.Text.Trim();

                // Gọi phương thức thực hiện tìm kiếm trong Controller
                _controller.SearchOnEnter(keyword, _controller.GetSearchCriterion(), PerformSearch);
            }
        }


        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.ShowAddNewForm(() => DataGridSection.Visibility = Visibility.Collapsed,
                                        () => AddNewForm.Visibility = Visibility.Visible);
            EditRecordID = -1;
        }

        private void ClearForm()
        {
            _controller.ClearFormFields(() =>
            {

                TenPhanBonTextBox.Clear();
                ThongTinTextBox.Clear();
                CoSoSanXuatTextBox.Clear();
                CoSoBuonBanTextBox.Clear();
            });
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy dữ liệu từ các trường nhập liệu
                string tenPhanBon = TenPhanBonTextBox.Text.Trim();
                string thongTin = ThongTinTextBox.Text.Trim();
                string coSoSanXuat = CoSoSanXuatTextBox.Text.Trim();
                string coSoBuonBan = CoSoBuonBanTextBox.Text.Trim();

                // Kiểm tra dữ liệu nhập vào
                if (string.IsNullOrWhiteSpace(tenPhanBon) || string.IsNullOrWhiteSpace(coSoSanXuat) || string.IsNullOrWhiteSpace(coSoBuonBan))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Gọi Controller để lưu dữ liệu
                _controller.SaveRecord(tenPhanBon, thongTin, coSoSanXuat, coSoBuonBan, EditRecordID,
                    () =>
                    {
                        LoadPhanBonData(); // Tải lại dữ liệu
                        MessageBox.Show(EditRecordID == -1 ? "Thêm mới thành công!" : "Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        CancelButton_Click(null, null); // Quay lại bảng
                    },
                    () => CancelButton_Click(null, null)); // Quay lại bảng
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Gọi phương thức từ Controller để xử lý quay lại DataGrid và ẩn form
                _controller.CancelEditing(ClearForm, () =>
                {
                    // Hiển thị DataGrid và ẩn form
                    DataGridSection.Visibility = Visibility.Visible;
                    AddNewForm.Visibility = Visibility.Collapsed;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi hủy: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Phương thức ClearForm để xóa dữ liệu trên form



        // Khi nhấn nút Xóa
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy ID của bản ghi từ Tag của nút
                int id = Convert.ToInt32((sender as Button).Tag);

                // Gọi phương thức từ Controller để xóa bản ghi
                _controller.DeleteRecord(id, LoadPhanBonData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi xóa: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int EditRecordID = -1;

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy ID của bản ghi từ Tag của nút
                int id = Convert.ToInt32((sender as Button).Tag);

                // Gọi phương thức từ Controller để chỉnh sửa bản ghi
                EditRecordID = _controller.EditRecord(id, (tenPhanBon, thongTin, coSoSanXuat, coSoBuonBan) =>
                {
                    // Hiển thị thông tin bản ghi lên form chỉnh sửa
                    TenPhanBonTextBox.Text = tenPhanBon;
                    ThongTinTextBox.Text = thongTin;
                    CoSoSanXuatTextBox.Text = coSoSanXuat;
                    CoSoBuonBanTextBox.Text = coSoBuonBan;

                    // Ẩn DataGrid và hiển thị form chỉnh sửa
                    DataGridSection.Visibility = Visibility.Collapsed;
                    AddNewForm.Visibility = Visibility.Visible;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi chỉnh sửa: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



    }
}