using PlantManagement.Controllers;
using PlantManagement.Helpers;
using System;
using System.Collections.Generic;
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

namespace PlantManagement.Views
{
    /// <summary>
    /// Interaction logic for QuanLyThuocBaoVeView.xaml
    /// </summary>
    public partial class QuanLyThuocBaoVeView : UserControl
    {
        private readonly ThuocBaoVeThucVatController _thuocBaoVeThucVatController;
        private readonly RoleController _roleController;
        public QuanLyThuocBaoVeView()
        {
            InitializeComponent();
            _thuocBaoVeThucVatController = new ThuocBaoVeThucVatController();
            _roleController = new RoleController();
            Loaded += QuanLyThuocBVTVView_Loaded;
        }
        private void QuanLyThuocBVTVView_Loaded(object sender, RoutedEventArgs e)
        {
            VaiTro(sender, e);
            LoadDataGridThuocBVTV();
        }
        public void VaiTro(object sender, RoutedEventArgs e)
        {
            string username = UserSession.Username;
            string role = _roleController.GetRoleUser(username);
            if (role != "Admin")
            {
                ButtonAdd.Visibility = Visibility.Collapsed;
                // Ẩn cột thao tác trong DataGrid nếu không phải admin
                var actionColumn = dataGridThuocBVTV.Columns
                    .FirstOrDefault(c => c.Header.ToString() == "Thao tác");

                if (actionColumn != null)
                {
                    actionColumn.Visibility = Visibility.Collapsed; // Ẩn cột thao tác
                }

            }
        }
        private void LoadDataGridThuocBVTV(string searchString = null)
        {
            DataTable dt = _thuocBaoVeThucVatController.GetAllThuocBVTVs(searchString);
            dataGridThuocBVTV.ItemsSource = dt.DefaultView;
        }
        private void LoadDataGridCSSX(string searchString = null)
        {
            DataTable dt = _thuocBaoVeThucVatController.GetAllCSSXs(searchString);
            dataGridCoSoSanXuat.ItemsSource = dt.DefaultView;
        }
        private void LoadDataGridCSBB(string searchString = null)
        {
            DataTable dt = _thuocBaoVeThucVatController.GetAllCSBBs(searchString);
            dataGridCoSoBuonBan.ItemsSource = dt.DefaultView;
        }
        // sự kiện nút combobox
        private void comboBox1_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (cbLoaiDanhMuc.SelectedItem is ComboBoxItem selectedItem)
            {
                // Lấy nội dung của mục được chọn
                string selectedContent = selectedItem.Content.ToString();

                // Ẩn tất cả DataGrid
                dataGridThuocBVTV.Visibility = Visibility.Collapsed;
                dataGridCoSoSanXuat.Visibility = Visibility.Collapsed;
                dataGridCoSoBuonBan.Visibility = Visibility.Collapsed;
                // [...] còn phần bản đồ trong combobox này nữa

                // Hiển thị DataGrid tương ứng
                switch (selectedContent)
                {
                    case "Thuốc bảo vệ thực vật":
                        dataGridThuocBVTV.Visibility = Visibility.Visible;
                        LoadDataGridThuocBVTV();
                        break;
                    case "Cơ sở sản xuất":
                        dataGridCoSoSanXuat.Visibility = Visibility.Visible;
                        LoadDataGridCSSX();
                        break;
                    case "Cơ sở buôn bán":
                        dataGridCoSoBuonBan.Visibility = Visibility.Visible;
                        LoadDataGridCSBB();
                        break;
                    case "Bản đồ":

                        break;
                    default:
                        break;
                }
            }
        }
        // Thao tác tìm kiếm
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy từ khóa tìm kiếm, loại bỏ khoảng trắng
            string keyword = txtSearch.Text.Trim();
            string selectedOption = cbLoaiDanhMuc.Text;

            // Xác định DataGrid cần thao tác
            DataGrid targetDataGrid = null;
            DataTable dataSource = null;

            switch (selectedOption)
            {
                case "Cơ sở sản xuất":
                    targetDataGrid = dataGridCoSoSanXuat;
                    dataSource = _thuocBaoVeThucVatController.GetAllCSSXs();
                    break;

                case "Cơ sở buôn bán":
                    targetDataGrid = dataGridCoSoBuonBan;
                    dataSource = _thuocBaoVeThucVatController.GetAllCSBBs();
                    break;
                default:
                    targetDataGrid = dataGridThuocBVTV;
                    dataSource = _thuocBaoVeThucVatController.GetAllThuocBVTVs();
                    break;
            }
            // Ẩn tất cả các DataGrid, chỉ hiển thị DataGrid đang xử lý
            dataGridThuocBVTV.Visibility = Visibility.Collapsed;
            dataGridCoSoSanXuat.Visibility = Visibility.Collapsed;
            dataGridCoSoBuonBan.Visibility = Visibility.Collapsed;
            targetDataGrid.Visibility = Visibility.Visible;

            // Kiểm tra nếu từ khóa rỗng thì hiển thị lại dữ liệu gốc
            if (string.IsNullOrWhiteSpace(keyword))
            {
                targetDataGrid.ItemsSource = dataSource.DefaultView; // Dữ liệu gốc
                return;
            }

            // Lọc dữ liệu
            var filteredRows = dataSource.AsEnumerable()
                .Where(row => row.ItemArray.Any(field =>
                    field != null && field.ToString()
                        .IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0));

            if (filteredRows != null && filteredRows.Any())
            {
                // Nếu có kết quả, tạo DataTable mới
                DataTable filteredDataTable = filteredRows.CopyToDataTable();
                targetDataGrid.ItemsSource = filteredDataTable.DefaultView; // Hiển thị kết quả
            }
            else
            {
                // Nếu không có kết quả, xóa dữ liệu trong DataGrid
                targetDataGrid.ItemsSource = null;
            }
        }
        // nút tất cả
        private void ShowAllButton_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            if (cbLoaiDanhMuc.SelectedIndex == 0)
            {
                dataGridThuocBVTV.Visibility = Visibility.Visible;
                LoadDataGridThuocBVTV();
            }
            else if (cbLoaiDanhMuc.SelectedIndex == 1)
            {
                dataGridCoSoSanXuat.Visibility = Visibility.Visible;
                LoadDataGridCSSX();
            }
            else if (cbLoaiDanhMuc.SelectedIndex == 2)
            {
                dataGridCoSoBuonBan.Visibility = Visibility.Visible;
                LoadDataGridCSBB();
            }
        }
        //sự kiện nút Thêm mới
        private void AddThuocBVTV_Click(object sender, EventArgs e)
        {
            MainGrid.Visibility = Visibility.Collapsed;
            AddGrid.Visibility = Visibility.Visible;
        }
        // nút cancelbutton
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            AddGrid.Visibility = Visibility.Collapsed; // Ẩn giao diện thêm mới
            MainGrid.Visibility = Visibility.Visible;  // Hiện giao diện chính
        }
        private void DeleteThuocBVTVButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridThuocBVTV.SelectedItem is DataRowView selectedRow)
            {
                // Lấy ID từ dòng được chọn
                int id = Convert.ToInt32(selectedRow["ID"]);

                // Hiển thị hộp thoại xác nhận xóa
                var result = MessageBox.Show(
                    "Bạn có chắc chắn muốn xóa thuốc bảo vệ thực vật này?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Gọi phương thức xóa trong Controller
                    _thuocBaoVeThucVatController.DeleteThuocBVTV(id);

                    // Tải lại DataGrid để hiển thị dữ liệu mới
                    LoadDataGridThuocBVTV();

                    // Hiển thị thông báo thành công
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void UpdateThuocBVTVButton_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra xem có dòng nào đang được chọn trong DataGrid hay không
            if (dataGridThuocBVTV.SelectedItem is DataRowView selectedRow)
            {
                // Duyệt qua các cột trong DataGrid để bật chế độ chỉnh sửa
                foreach (DataGridColumn column in dataGridThuocBVTV.Columns)
                {
                    if (column is DataGridTextColumn textColumn)
                    {
                        textColumn.IsReadOnly = false;  // Chuyển chế độ đọc/ghi cho các cột
                    }
                }

                // Nếu cần, bạn có thể thay đổi màu sắc của hàng đang được chỉnh sửa hoặc thêm một số thay đổi giao diện khác để dễ nhận biết
                selectedRow.BeginEdit();  // Bắt đầu chế độ chỉnh sửa cho dòng đã chọn
            }
        }
        private void SaveThuocBVTVButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy toàn bộ dữ liệu từ DataGrid
            foreach (var item in dataGridThuocBVTV.Items)
            {
                if (item is DataRowView row)
                {
                    int id = Convert.ToInt32(row["ID"]);
                    string tenThuoc = row["TenThuocBVTV"].ToString();
                    string thongTin = row["ThongTinThuoc"].ToString();
                    string tenCSSX = row["TenCoSoSanXuat"].ToString();
                    string diaChiCSSX = row["DiaChiCoSoSanXuat"].ToString();
                    string tenCSBB = row["TenCoSoBuonBan"].ToString();
                    string diaChiCSBB = row["DiaChiCoSoBuonBan"].ToString();

                    // Gọi hàm cập nhật từ Controller
                    bool isUpdated = _thuocBaoVeThucVatController.UpdateThuocBVTV(id, tenThuoc, thongTin, tenCSSX, diaChiCSSX, tenCSBB, diaChiCSBB);

                    // Xử lý kết quả
                    if (!isUpdated)
                    {
                        MessageBox.Show($"Cập nhật thất bại cho ID: {id}!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy dữ liệu từ các TextBox
            string tenThuoc = TenTextBox.Text.Trim();
            string thongTin = ThongTinTextBox.Text.Trim();
            string tenCSSX = CSSXTextBox.Text.Trim();
            string diaChiCSSX = DCCSSXTextBox.Text.Trim();
            string tenCSBB = CSBBTextBox.Text.Trim();
            string diaChiCSBB = DCCSBBTextBox.Text.Trim();

            // Kiểm tra dữ liệu hợp lệ
            if (string.IsNullOrWhiteSpace(tenThuoc) || string.IsNullOrWhiteSpace(tenCSSX) || string.IsNullOrWhiteSpace(tenCSBB))
            {
                MessageBox.Show("Tên thuốc, cơ sở sản xuất và cơ sở buôn bán không được để trống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra định dạng địa chỉ (phải có đủ 3 thành phần: Xã, Huyện, Tỉnh)
            if (!IsValidDiaChi(diaChiCSSX) || !IsValidDiaChi(diaChiCSBB))
            {
                MessageBox.Show("Địa chỉ phải có định dạng: Xã, Huyện, Tỉnh!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Gửi dữ liệu đến Controller để lưu
            bool isSaved = _thuocBaoVeThucVatController.AddThuocBVTV(tenThuoc, thongTin, tenCSSX, diaChiCSSX, tenCSBB, diaChiCSBB);

            // Hiển thị thông báo
            if (isSaved)
            {
                MessageBox.Show("Thêm mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                // Load lại datagrid
                LoadDataGridThuocBVTV();
                // Làm sạch các TextBox sau khi lưu thành công
                TenTextBox.Text = string.Empty;
                ThongTinTextBox.Text = string.Empty;
                CSSXTextBox.Text = string.Empty;
                DCCSSXTextBox.Text = string.Empty;
                CSBBTextBox.Text = string.Empty;
                DCCSBBTextBox.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("Thêm mới thất bại! Vui lòng thử lại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Hàm kiểm tra định dạng địa chỉ
        private bool IsValidDiaChi(string diaChi)
        {
            // Kiểm tra xem địa chỉ có đủ 3 thành phần cách nhau bằng dấu phẩy hay không
            string[] parts = diaChi.Split(',');
            return parts.Length == 3 && parts.All(part => !string.IsNullOrWhiteSpace(part.Trim()));
        }
    }

}