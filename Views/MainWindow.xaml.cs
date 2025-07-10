using System.Windows;
using PlantManagement.Controllers;
using PlantManagement.Helpers;
using PlantManagement.Views;

namespace PlantManagement.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainController _controller;
        private bool _isAdmin; // Biến _isAdmin xác định nếu người dùng là Admin
        private User currentUser; // Thêm thuộc tính lưu thông tin người dùng

        public MainWindow()
        {
            InitializeComponent();
            _controller = new MainController();
            _isAdmin = false; // Mặc định là không phải admin
        }

        // Khi nhấn vào nút Đăng Nhập
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            if (loginWindow.ShowDialog() == true)
            {
                // Lấy thông tin người dùng từ cửa sổ đăng nhập
                currentUser = loginWindow.GetCurrentUser();

                // Kiểm tra ID_Role để xác định người dùng có phải là Admin hay không
                if (currentUser.ID_Role == 1)
                {
                    _isAdmin = true; // Admin
                }
                else if (currentUser.ID_Role == 2)
                {
                    _isAdmin = false; // User
                }

                LoadMainUI();
                UpdateUserName(currentUser.FullName); // Hiển thị tên người dùng sau khi đăng nhập
            }
            else
            {
                MessageBox.Show("Đăng nhập thất bại! Vui lòng thử lại.");
            }
        }

        // Khi nhấn vào nút Đăng Ký
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }

        // Tải giao diện sau khi đăng nhập thành công
        private void LoadMainUI()
        {
            // Ẩn giao diện đăng nhập và hiển thị giao diện chính
            InitialPanel.Visibility = Visibility.Collapsed;
            MainPanel.Visibility = Visibility.Visible;

            // Kiểm tra quyền để ẩn/hiện các nút chức năng
            if (_isAdmin)
            {
                // Hiển thị tất cả các nút khi là Admin
                ManageUserButton.Visibility = Visibility.Visible;
                ManageAdministrativeUnitButton.Visibility = Visibility.Visible;
                ManagePlantVarietyButton.Visibility = Visibility.Visible;
                ManagePesticidesButton.Visibility = Visibility.Visible;
                ManageFertilizersButton.Visibility = Visibility.Visible;
                ManageProductionButton.Visibility = Visibility.Visible;
                ReportButton.Visibility = Visibility.Visible;
            }
            else
            {
                // Ẩn các nút không cần thiết khi là User
                ManageUserButton.Visibility = Visibility.Collapsed;
                ManageAdministrativeUnitButton.Visibility = Visibility.Visible;
                ManagePlantVarietyButton.Visibility = Visibility.Visible;
                ManagePesticidesButton.Visibility = Visibility.Visible;
                ManageFertilizersButton.Visibility = Visibility.Visible;
                ManageProductionButton.Visibility = Visibility.Visible;
                ReportButton.Visibility = Visibility.Collapsed;
            }
        }

        // Cập nhật tên người dùng khi đăng nhập thành công
        private void UpdateUserName(string userName)
        {
            
        }

        // Khi nhấn vào nút "My Account"
        private void MyAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser != null)
            {
                QuanLyThongTinTaiKhoanView accountView = new QuanLyThongTinTaiKhoanView(currentUser); // Tạo đối tượng "My Account"
                MainContent.Content = accountView; // Hiển thị trang "My Account"
            }
            else
            {
                MessageBox.Show("Vui lòng đăng nhập trước.");
            }
        }

        // Khi nhấn vào nút "Manage User"
        private void ManageUserButton_Click(object sender, RoutedEventArgs e)
        {
            QuanLyNguoiDungView userManagementView = new QuanLyNguoiDungView();
            MainContent.Content = userManagementView; // Hiển thị trang quản lý người dùng
        }

        // Khi nhấn vào nút "Manage Administrative Unit"
        private void ManageAdministrativeUnitButton_Click(object sender, RoutedEventArgs e)
        {
            DonViHanhChinhView donViHanhChinhView = new DonViHanhChinhView();
            MainContent.Content = donViHanhChinhView;
        }

        // Khi nhấn vào nút "Manage Plant Variety"
        private void ManagePlantVarietyButton_Click(object sender, RoutedEventArgs e)
        {
            QuanLyGiongCayTrongView quanLyGiongCayTrongView = new QuanLyGiongCayTrongView();
            MainContent.Content = quanLyGiongCayTrongView;
        }

        // Khi nhấn vào nút "Manage Pesticides"
        private void ManagePesticidesButton_Click(object sender, RoutedEventArgs e)
        {
            QuanLyThuocBaoVeView quanLyThuocBaoVeView = new QuanLyThuocBaoVeView();
            MainContent.Content = quanLyThuocBaoVeView;
        }

        // Khi nhấn vào nút "Manage Fertilizers"
        private void ManageFertilizersButton_Click(object sender, RoutedEventArgs e)
        {
            QuanLyPhanBonView quanLyPhanBonView = new QuanLyPhanBonView();
            MainContent.Content = quanLyPhanBonView;
        }

        // Khi nhấn vào nút "Manage Production"
        private void ManageProductionButton_Click(object sender, RoutedEventArgs e)
        {
            QuanLySanXuatTrongTrotView quanLySanXuatTrongTrotView = new QuanLySanXuatTrongTrotView();
            MainContent.Content = quanLySanXuatTrongTrotView;
        }

        // Khi nhấn vào nút "Report"
        private void BaoCaoButton_Click(object sender, RoutedEventArgs e)
        {
            BaoCaoView baoCaoView = new BaoCaoView();
            MainContent.Content = baoCaoView; // Hiển thị trang Báo cáo
            // Tự động gọi sự kiện "NguoiDung_Click" khi giao diện Báo Cáo được mở
            baoCaoView.NguoiDung_Click(this, new RoutedEventArgs());
        }

        // Khi nhấn vào nút Đăng Xuất
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đăng xuất thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            // Quay lại giao diện đăng nhập
            UserSession.Username = null; // Xóa thông tin phiên
            MainPanel.Visibility = Visibility.Collapsed;
            InitialPanel.Visibility = Visibility.Visible;
        }
    }
}
