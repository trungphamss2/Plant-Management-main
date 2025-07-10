using System.Windows;
using PlantManagement.Controllers;

namespace PlantManagement.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginController _loginController;
        private User _currentUser;

        public LoginWindow()
        {
            InitializeComponent();
            _loginController = new LoginController();  // Khởi tạo đối tượng LoginController
        }

        public User GetCurrentUser() => _currentUser;

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy tên đăng nhập và mật khẩu từ giao diện người dùng
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            // Gọi phương thức AuthenticateUser để kiểm tra thông tin đăng nhập
            _currentUser = _loginController.AuthenticateUser(username, password);

            if (_currentUser != null)
            {
                // Lưu lịch sử đăng nhập thành công
                _loginController.SaveLoginHistory(_currentUser.ID, true);
                Application.Current.Resources["username"] = username; // Lưu vào Resources
            }
            if(_currentUser != null)
            {

            

                // Hiển thị thông báo đăng nhập thành công
                MessageBox.Show($"Đăng nhập thành công! Chào mừng {_currentUser.FullName}.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                this.Close(); // Đóng cửa sổ đăng nhập
            }
            else
            {
                // Hiển thị thông báo lỗi đăng nhập
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Đóng cửa sổ đăng nhập
        }

        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            // Ẩn phần đăng nhập, nút quên mật khẩu và hiển thị phần quên mật khẩu
            LoginPanel.Visibility = Visibility.Collapsed;
            ForgotPasswordPanel.Visibility = Visibility.Visible;
            ForgotPasswordButton.Visibility = Visibility.Collapsed; // Ẩn nút "Quên mật khẩu"

            // Làm mới các trường thông tin
            ForgotUsernameTextBox.Clear();
            ForgotEmailTextBox.Clear();
        }

        private void BackToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Quay lại giao diện đăng nhập
            ForgotPasswordPanel.Visibility = Visibility.Collapsed;
            LoginPanel.Visibility = Visibility.Visible;
            ForgotPasswordButton.Visibility = Visibility.Visible; // Hiển thị lại nút "Quên mật khẩu"
        }

        private void SubmitForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            string username = ForgotUsernameTextBox.Text;
            string email = ForgotEmailTextBox.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra tài khoản và email
            string password = _loginController.GetPasswordByUsernameAndEmail(username, email);

            if (!string.IsNullOrEmpty(password))
            {
                MessageBox.Show($"Mật khẩu của bạn là: {password}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Đóng cửa sổ hiện tại và mở trang chính
                this.Close(); // Đóng cửa sổ Quên mật khẩu

                MainWindow mainLogin = new MainWindow(); // Khởi tạo trang chính
                mainLogin.Show();
            }
            else
            {
                MessageBox.Show("Tài khoản hoặc email không chính xác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
