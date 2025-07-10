using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PlantManagement.Controllers;
using PlantManagement.Helpers;

namespace PlantManagement.Views
{
    public partial class QuanLyGiongCayTrongView : UserControl
    {
        private readonly GiongCayTrongController _giongCayTrongController;
        private readonly RoleController _roleController;

        public QuanLyGiongCayTrongView()
        {
            InitializeComponent();
            _giongCayTrongController = new GiongCayTrongController();
            _roleController = new RoleController();
            // Đảm bảo DataGrid được tải ngay khi khởi tạo giao diện
            Loaded += QuanLyGiongCayTrongView_Loaded;
        }
        private void QuanLyGiongCayTrongView_Loaded(object sender, RoutedEventArgs e)
        {
            VaiTro(sender, e);
            LoadDataGridGiongCayTrong();
        }
        public void VaiTro(object sender, RoutedEventArgs e)
        {
            string username = UserSession.Username;
            string role = _roleController.GetRoleUser(username);
            if (role == "Admin")
            {
                ButtonAdd.Visibility = Visibility.Visible;
                // Hiện cột thao tác trong DataGrid nếu là admin
                var actionColumn = dataGridGiongCayTrong.Columns
                    .FirstOrDefault(c => c.Header.ToString() == "Thao tác");

                if (actionColumn != null)
                {
                    actionColumn.Visibility = Visibility.Visible; // Hiện cột thao tác
                }

            }
            else
            {
                ButtonAdd.Visibility = Visibility.Collapsed;
                // Ẩn cột thao tác trong DataGrid nếu không phải admin
                var actionColumn = dataGridGiongCayTrong.Columns
                    .FirstOrDefault(c => c.Header.ToString() == "Thao tác");

                if (actionColumn != null)
                {
                    actionColumn.Visibility = Visibility.Collapsed; // Ẩn cột thao tác
                }
            }
        }

        // Hiển thị datagrid giống cây trồng 
        private void LoadDataGridGiongCayTrong(string searchString = null)
        {
            DataTable dt = _giongCayTrongController.GetAllGiongCayTrongs(searchString);
            dataGridGiongCayTrong.ItemsSource = dt.DefaultView; // Gán dữ liệu vào DataGrid
        }
        // Hiển thị datagrid giống cây trồng chính 
        private void LoadDataGridGiongCayTrongChinh(string searchString = null)
        {
            DataTable dt = _giongCayTrongController.GetAllGiongCayTrongChinh(searchString);
            dataGridGiongCayTrongChinh.ItemsSource = dt.DefaultView; // Gán dữ liệu vào DataGrid
        }
        // Hiển thị datagrid cây đầu dòng 
        private void LoadDataGridCayDauDong(string searchString = null)
        {
            DataTable dt = _giongCayTrongController.GetAllCayDauDong(searchString);
            dataGridCayDauDong.ItemsSource = dt.DefaultView; // Gán dữ liệu vào DataGrid
        }

        // Hiển thị datagrid vườn cây đầu dòng 
        private void LoadDataGridVuonCayDauDong(string searchString = null)
        {
            DataTable dt = _giongCayTrongController.GetAllVuonCayDauDong(searchString);
            dataGridVuonCayDauDong.ItemsSource = dt.DefaultView; // Gán dữ liệu vào DataGrid
        }
        // sự kiện khi bấm nút combobox
        private void comboBox1_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbLoaiDanhMuc.SelectedItem is ComboBoxItem selectedItem)
            {
                // Lấy nội dung của mục được chọn
                string selectedContent = selectedItem.Content.ToString();

                // Ẩn tất cả DataGrid
                dataGridGiongCayTrong.Visibility = Visibility.Collapsed;
                dataGridGiongCayTrongChinh.Visibility = Visibility.Collapsed;
                dataGridCayDauDong.Visibility = Visibility.Collapsed;
                dataGridVuonCayDauDong.Visibility = Visibility.Collapsed;

                // Hiển thị DataGrid tương ứng
                switch (selectedContent)
                {
                    case "Giống cây trồng":
                        dataGridGiongCayTrong.Visibility = Visibility.Visible;
                        LoadDataGridGiongCayTrong();
                        break;
                    case "Giống cây trồng chính":
                        dataGridGiongCayTrongChinh.Visibility = Visibility.Visible;
                        LoadDataGridGiongCayTrongChinh();
                        break;
                    case "Cây đầu dòng":
                        dataGridCayDauDong.Visibility = Visibility.Visible;
                        LoadDataGridCayDauDong();
                        break;
                    case "Vườn cây đầu dòng":
                        dataGridVuonCayDauDong.Visibility = Visibility.Visible;
                        LoadDataGridVuonCayDauDong();
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
                case "Giống cây trồng chính":
                    targetDataGrid = dataGridGiongCayTrongChinh;
                    dataSource = _giongCayTrongController.GetAllGiongCayTrongChinh();
                    break;

                case "Cây đầu dòng":
                    targetDataGrid = dataGridCayDauDong;
                    dataSource = _giongCayTrongController.GetAllCayDauDong();
                    break;

                case "Vườn cây đầu dòng":
                    targetDataGrid = dataGridVuonCayDauDong;
                    dataSource = _giongCayTrongController.GetAllVuonCayDauDong();
                    break;

                default:
                    targetDataGrid = dataGridGiongCayTrong;
                    dataSource = _giongCayTrongController.GetAllGiongCayTrongs();
                    break;
            }
            // Ẩn tất cả các DataGrid, chỉ hiển thị DataGrid đang xử lý
            dataGridGiongCayTrong.Visibility = Visibility.Collapsed;
            dataGridGiongCayTrongChinh.Visibility = Visibility.Collapsed;
            dataGridCayDauDong.Visibility = Visibility.Collapsed;
            dataGridVuonCayDauDong.Visibility = Visibility.Collapsed;
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
        // Hiển thị tất cả dữ liệu
        private void ShowAllButton_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            if (cbLoaiDanhMuc.SelectedIndex == -1 || cbLoaiDanhMuc.SelectedIndex == 0)
            {
                dataGridGiongCayTrong.Visibility = Visibility.Visible;
                LoadDataGridGiongCayTrong();
            }
            else if (cbLoaiDanhMuc.SelectedIndex == 1)
            {
                dataGridGiongCayTrongChinh.Visibility = Visibility.Visible;
                LoadDataGridGiongCayTrongChinh();
            }
            else if (cbLoaiDanhMuc.SelectedIndex == 2)
            {
                dataGridCayDauDong.Visibility = Visibility.Visible;
                LoadDataGridCayDauDong();
            }
            else if (cbLoaiDanhMuc.SelectedIndex == 3)
            {
                dataGridVuonCayDauDong.Visibility = Visibility.Visible;
                LoadDataGridVuonCayDauDong();
            }

        }
        // Xóa giống cây trồng
        private void DeleteGiongCayTrongButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridGiongCayTrong.SelectedItem is DataRowView selectedRow)
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
                    _giongCayTrongController.DeleteGiongCayTrong(id);

                    // Tải lại DataGrid để hiển thị dữ liệu mới
                    LoadDataGridGiongCayTrong();

                    // Hiển thị thông báo thành công
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        // cập nhật giống cây trồng
        private void UpdateGiongCayTrongButton_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra xem có dòng nào đang được chọn trong DataGrid hay không
            if (dataGridGiongCayTrong.SelectedItem is DataRowView selectedRow)
            {
                // Duyệt qua các cột trong DataGrid để bật chế độ chỉnh sửa
                foreach (DataGridColumn column in dataGridGiongCayTrong.Columns)
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
        // lưu giống cây trồng sau khi cập nhật
        private void SaveGiongCayTrongButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridGiongCayTrong.SelectedItem is DataRowView selectedRow)
            {
                try
                {
                    // Lấy dữ liệu từ các cột trong DataGrid
                    int id = Convert.ToInt32(selectedRow["ID"]);
                    string tenGiong = selectedRow["TenGiong"]?.ToString() ?? string.Empty;
                    string moTa = selectedRow["MoTa"]?.ToString() ?? string.Empty;
                    string loaiCay = selectedRow["LoaiCay"]?.ToString() ?? string.Empty;

                    // Lấy nơi lưu hành từ DataGrid (danh sách các đơn vị hành chính ID, cách nhau bởi dấu phẩy)
                    string dsNoiLuuHanhRaw = selectedRow["NoiLuuHanh"]?.ToString() ?? string.Empty;
                    List<string> dsNoiLuuHanh = string.IsNullOrWhiteSpace(dsNoiLuuHanhRaw)
                        ? new List<string>() // Nếu chuỗi rỗng, trả về danh sách trống
                        : dsNoiLuuHanhRaw
                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim()) // Xóa khoảng trắng thừa
                            .ToList();

                    // Lấy danh sách vườn cây đầu dòng từ DataGrid (danh sách các vườn cây, cách nhau bởi dấu phẩy)
                    string dsVuonCayRaw = selectedRow["TenVuonCay"]?.ToString() ?? string.Empty;
                    List<string> dsVuonCay = string.IsNullOrWhiteSpace(dsVuonCayRaw)
                        ? new List<string>() // Nếu chuỗi rỗng, trả về danh sách trống
                        : dsVuonCayRaw
                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim()) // Xóa khoảng trắng thừa
                            .ToList();

                    // Lấy danh sách địa chỉ vườn cây đầu dòng
                    string dsdiaChiVuonCayRaw = selectedRow["DiaChiVuonCay"]?.ToString() ?? string.Empty;
                    List<string> dsdiaChiVuonCay = string.IsNullOrWhiteSpace(dsdiaChiVuonCayRaw)
                        ? new List<string>() // Nếu chuỗi rỗng, trả về danh sách trống
                        : dsdiaChiVuonCayRaw
                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim()) // Xóa khoảng trắng thừa
                            .ToList();


                    // Thực hiện các thao tác cập nhật dữ liệu
                    bool isUpdated = _giongCayTrongController.UpdateGiongCayTrong(
                        id,
                        tenGiong,
                        moTa,
                        loaiCay,
                        dsNoiLuuHanh, // Truyền danh sách nơi lưu hành
                        dsVuonCay,    // Truyền danh sách vườn cây đầu dòng
                        dsdiaChiVuonCay// Truyền địa chỉ vườn cây
                    );

                    if (isUpdated)
                    {
                        MessageBox.Show("Cập nhật giống cây trồng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDataGridGiongCayTrong(); // Tải lại dữ liệu trong DataGrid
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật giống cây trồng thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một giống cây trồng để cập nhật.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        //sự kiện nút Thêm mới
        private void AddGiongCayTrong_Click(object sender, EventArgs e)
        {
            MainGrid.Visibility = Visibility.Collapsed;
            AddGrid.Visibility = Visibility.Visible;
        }
        // Thêm một nơi lưu hành mới

        private void AddNoiLuuHanhButton_Click(object sender, RoutedEventArgs e)
        {
            // Tạo StackPanel mới cho nơi lưu hành
            StackPanel newLuuHanhPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(5) };
            // Thêm mô tả hướng dẫn

            TextBlock descriptionTextBlock = new TextBlock
            {
                Text = "Điền địa chỉ theo hướng dẫn sau: Tên xã, Tên huyện, Tên tỉnh. Ví dụ: A, B, C",
                FontSize = 10,
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(0, 5, 0, 5),
            };

            // Tạo TextBox mới
            TextBox newNoiLuuHanhTextBox = new TextBox
            {
                Margin = new Thickness(0, 5, 0, 5),
                Height = 18
            };

            // Tạo nút xóa
            Button deleteButton = new Button
            {
                Content = "X",
                Width = 30,
                Margin = new Thickness(0, 5, 0, 5),
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = Brushes.Red,
                Foreground = Brushes.White
            };
            // Sự kiện xóa khi bấm nút
            deleteButton.Click += (s, args) =>
            {
                NhomLuuHanhStackPanel.Children.Remove(newLuuHanhPanel);
            };

            // Thêm TextBox và nút xóa vào StackPanel
            newLuuHanhPanel.Children.Add(descriptionTextBlock);
            newLuuHanhPanel.Children.Add(newNoiLuuHanhTextBox);
            newLuuHanhPanel.Children.Add(deleteButton);

            // Thêm StackPanel vào NhomLuuHanhStackPanel
            NhomLuuHanhStackPanel.Children.Add(newLuuHanhPanel);
        }

        // Xử lý thêm vườn cây đầu dòng mới
        private void AddVuonCayDauDongButton_Click(object sender, RoutedEventArgs e)
        {
            // Tạo StackPanel mới cho vườn cây
            StackPanel newVuonCayPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(5) };
            // Thêm nhãn tên vườn cây
            TextBlock tenVuonLabel = new TextBlock
            {
                Text = "Tên vườn cây",
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 5, 0, 5),
                Foreground = Brushes.Gray
            };
            // Tạo TextBox cho tên vườn cây
            TextBox newVuonCayTextBox = new TextBox
            {
                Margin = new Thickness(0, 5, 0, 5),
                Height = 18
            };
            // Thêm mô tả hướng dẫn
            TextBlock descriptionTextBlock = new TextBlock
            {
                Text = "Điền địa chỉ theo hướng dẫn sau: Tên xã, Tên huyện, Tên tỉnh. Ví dụ: A, B, C",
                FontSize = 10,
                FontStyle = FontStyles.Italic,

            };

            // Tạo TextBox cho địa chỉ
            TextBox newDiaChiTextBox = new TextBox
            {
                Margin = new Thickness(0, 5, 0, 5),
                Height = 18
            };

            // Tạo nút xóa
            Button deleteButton = new Button
            {
                Content = "X",
                Width = 30,
                Margin = new Thickness(0, 5, 0, 5),
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = Brushes.Red,
                Foreground = Brushes.White
            };
            // Sự kiện xóa khi bấm nút
            deleteButton.Click += (s, args) =>
            {
                VuonCayDauDongStackPanel.Children.Remove(newVuonCayPanel);
            };

            // Thêm TextBox và nút xóa vào StackPanel
            newVuonCayPanel.Children.Add(tenVuonLabel);
            newVuonCayPanel.Children.Add(newVuonCayTextBox);
            newVuonCayPanel.Children.Add(descriptionTextBlock);
            newVuonCayPanel.Children.Add(newDiaChiTextBox);
            newVuonCayPanel.Children.Add(deleteButton);

            // Thêm StackPanel vào VuonCayDauDongStackPanel
            VuonCayDauDongStackPanel.Children.Add(newVuonCayPanel);
        }

        // nút lưu giao diện thêm mới
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tenGiong = TenTextBox.Text.Trim();
                string moTa = MoTaTextBox.Text.Trim();
                string loaiCay = LoaiCayComboBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(tenGiong) || string.IsNullOrWhiteSpace(loaiCay))
                {
                    MessageBox.Show("Tên giống cây và loại cây không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var noiLuuHanhList = new List<string>();
                foreach (var child in NhomLuuHanhStackPanel.Children)
                {
                    if (child is TextBox textBox && !string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        noiLuuHanhList.Add(textBox.Text.Trim());
                    }
                }

                var tenVuonCayList = new List<string>();
                var diaChiVuonCayList = new List<string>();
                foreach (var child in VuonCayDauDongStackPanel.Children)
                {
                    if (child is StackPanel panel)
                    {
                        var textBoxes = panel.Children.OfType<TextBox>().ToList();
                        if (textBoxes.Count == 2)
                        {
                            var tenVuonTextBox = textBoxes[0];
                            var diaChiTextBox = textBoxes[1];

                            if (!string.IsNullOrWhiteSpace(tenVuonTextBox.Text) && !string.IsNullOrWhiteSpace(diaChiTextBox.Text))
                            {
                                if (!IsValidDiaChi(diaChiTextBox.Text))
                                {
                                    MessageBox.Show("Địa chỉ vườn cây phải có định dạng: Xã, Huyện, Tỉnh!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }

                                tenVuonCayList.Add(tenVuonTextBox.Text.Trim());
                                diaChiVuonCayList.Add(diaChiTextBox.Text.Trim());
                            }
                            else
                            {
                                MessageBox.Show("Vui lòng nhập đầy đủ tên vườn cây và địa chỉ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }
                    }
                }



                var controller = new GiongCayTrongController();
                bool isSuccess = controller.AddGiongCayTrong(tenGiong, moTa, loaiCay, noiLuuHanhList, tenVuonCayList, diaChiVuonCayList);

                if (isSuccess)
                {
                    MessageBox.Show("Thêm giống cây trồng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadDataGridGiongCayTrong();
                    // Xóa nội dung trong các TextBox và ComboBox
                    TenTextBox.Clear();
                    MoTaTextBox.Clear();
                    LoaiCayComboBox.SelectedIndex = -1;
                    foreach (var child in NhomLuuHanhStackPanel.Children.OfType<TextBox>())
                    {
                        child.Clear();
                    }
                    foreach (var child in VuonCayDauDongStackPanel.Children.OfType<TextBox>())
                    {
                        child.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("Thêm giống cây trồng thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Hàm kiểm tra định dạng địa chỉ
        private bool IsValidDiaChi(string diaChi)
        {
            // Kiểm tra xem địa chỉ có đủ 3 thành phần cách nhau bằng dấu phẩy hay không
            string[] parts = diaChi.Split(',');
            return parts.Length == 3 && parts.All(part => !string.IsNullOrWhiteSpace(part.Trim()));
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            AddGrid.Visibility = Visibility.Collapsed; // Ẩn giao diện thêm mới
            MainGrid.Visibility = Visibility.Visible;  // Hiện giao diện chính
        }
    }
}