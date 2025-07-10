using System;
using System.Windows;
using PlantManagement.Controllers;

namespace PlantManagement.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly RegisterController _registerController;

        public RegisterWindow()
        {
            InitializeComponent();
            _registerController = new RegisterController();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy dữ liệu từ giao diện
                string username = UsernameTextBox.Text.Trim();
                string fullName = FullNameTextBox.Text.Trim();
                string email = EmailTextBox.Text.Trim();
                string password = PasswordBox.Password;
                string confirmPassword = ConfirmPasswordBox.Password;

                // Kiểm tra tính hợp lệ
                string validationMessage = ValidateInputs(username, fullName, email, password, confirmPassword);
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    MessageBox.Show(validationMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Gọi phương thức Register trong RegisterController
                bool success = _registerController.Register(username, fullName, email, password);

                if (success)
                {
                    MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập lại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Đóng cửa sổ đăng ký và chuyển về cửa sổ đăng nhập
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Đăng ký thất bại! Tên đăng nhập đã tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của thông tin đầu vào.
        /// </summary>
        private string ValidateInputs(string username, string fullName, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(fullName) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                return "Vui lòng điền đầy đủ thông tin.";
            }

            if (!IsValidEmail(email))
            {
                return "Email không hợp lệ.";
            }

            if (password != confirmPassword)
            {
                return "Mật khẩu và xác nhận mật khẩu không khớp.";
            }

            if (password.Length < 6) // Ví dụ: yêu cầu mật khẩu tối thiểu 6 ký tự
            {
                return "Mật khẩu phải có ít nhất 6 ký tự.";
            }

            return null; // Không có lỗi
        }

        /// <summary>
        /// Kiểm tra định dạng email hợp lệ.
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
