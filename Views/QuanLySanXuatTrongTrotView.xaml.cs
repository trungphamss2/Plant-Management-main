using PlantManagement.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using PlantManagement.Controllers;

namespace PlantManagement.Views
{
    /// <summary>
    /// Interaction logic for QuanLySanXuatTrongTrotView.xaml
    /// </summary>
    public partial class QuanLySanXuatTrongTrotView : UserControl
    {
        private DatabaseHelper _databaseHelper;
        private QuanLySanXuatTrongTrotController _controller;

        public QuanLySanXuatTrongTrotView()
        {
            InitializeComponent();
            _controller = new QuanLySanXuatTrongTrotController();
            LoadCoSoData();
        }
        private void LoadCoSoData()
        {
            var dataTable = _controller.GetAllSXTT();
            DataGridSXTT.ItemsSource = dataTable.DefaultView;
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Gọi phương thức ToggleSearchOptionsVisibility từ Controller
            SearchOptions.Visibility = _controller.ToggleSearchOptionsVisibility(SearchOptions.Visibility);
        }

      
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Gọi phương thức từ Controller để reset tìm kiếm
                _controller.ResetSearch(LoadCoSoData);

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
                DataGridSXTT.ItemsSource = dataTable.DefaultView;
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
                VietGAPCheckBox.IsChecked = false;
                NotVietGAPCheckBox.IsChecked = false;
                SanPhamTextBox.Clear();
                VungTrongTextBox.Clear();
                CoSoTextBox.Clear();
                SinhVatGayHaiTextBox.Clear();
            });
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Truy cập các giá trị từ checkbox và các trường nhập liệu
                bool vietGap = VietGAPCheckBox.IsChecked == true;
                string sanPham = SanPhamTextBox.Text;
                string coso = CoSoTextBox.Text;
                string vungTrong = VungTrongTextBox.Text;
                string sinhVatGayHai = SinhVatGayHaiTextBox.Text;

                // Kiểm tra các giá trị nhập vào
                if (string.IsNullOrWhiteSpace(sanPham) || string.IsNullOrWhiteSpace(vungTrong) || string.IsNullOrWhiteSpace(sinhVatGayHai))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Gọi phương thức từ Controller để lưu dữ liệu
                _controller.SaveRecord(vietGap, sanPham, coso, vungTrong, sinhVatGayHai, EditRecordID,
                    () =>
                    {
                        // Callback sau khi thành công
                        LoadCoSoData(); // Tải lại dữ liệu
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

        // Khi nhấn nút Xóa
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy ID của bản ghi từ Tag của nút
                int id = Convert.ToInt32((sender as Button).Tag);

                // Gọi phương thức từ Controller để xóa bản ghi
                _controller.DeleteRecord(id, LoadCoSoData);
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
                EditRecordID = _controller.EditRecord(id, (sanPham, coSo, sinhVatGayHai, vungTrong, vietGap) =>
                {
                    // Hiển thị thông tin bản ghi lên form chỉnh sửa
                    SanPhamTextBox.Text = sanPham;
                    CoSoTextBox.Text = coSo;
                    SinhVatGayHaiTextBox.Text = sinhVatGayHai;
                    VungTrongTextBox.Text = vungTrong;
                    VietGAPCheckBox.IsChecked = vietGap;

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

        private void DataGridSXTT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
