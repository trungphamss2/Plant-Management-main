using System.Windows;
using System.Windows.Controls;
using PlantManagement.Models;
using PlantManagement.Controllers;

namespace PlantManagement.Views
{
    public partial class ResetPasswordView : UserControl
    {
        private User _currentUser;
        private ResetPasswordController _resetPasswordController;

        public ResetPasswordView(User user)
        {
            InitializeComponent();
            _currentUser = user;
            _resetPasswordController = new ResetPasswordController();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy thông tin từ giao diện
            string oldPassword = OldPasswordBox.Password;
            string newPassword = NewPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            // Kiểm tra thông tin nhập liệu
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu mới và xác nhận mật khẩu không khớp.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Thực hiện đổi mật khẩu
            bool isPasswordUpdated = _resetPasswordController.UpdatePassword(_currentUser.ID, oldPassword, newPassword);
            if (isPasswordUpdated)
            {
                MessageBox.Show("Mật khẩu đã được thay đổi thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                BackToAccountInfo(); // Quay lại màn hình thông tin tài khoản
            }
            else
            {
                MessageBox.Show("Mật khẩu cũ không đúng hoặc lỗi hệ thống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackToAccountInfo(); // Quay lại màn hình thông tin tài khoản
        }

        private void BackToAccountInfo()
        {
            // Trở về giao diện thông tin tài khoản
            QuanLyThongTinTaiKhoanView accountInfoView = new QuanLyThongTinTaiKhoanView(_currentUser);
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = accountInfoView;
        }
    }
}