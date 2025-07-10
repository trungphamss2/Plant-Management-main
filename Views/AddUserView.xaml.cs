using PlantManagement.Controllers;
using PlantManagement.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PlantManagement.Views
{
    public partial class AddUserView : UserControl
    {
        public event Action OnUserAdded; // Sự kiện để thông báo khi thêm người dùng thành công
        private UserControl _previousUserControl; // Biến lưu UserControl trước đó

        // Constructor nhận UserControl trước đó
        public AddUserView(UserControl previousUserControl)
        {
            InitializeComponent();
            _previousUserControl = previousUserControl; // Lưu UserControl trước đó
            LoadRoles(); // Tải các vai trò vào ComboBox khi khởi tạo
        }

        // Phương thức để tải danh sách vai trò từ cơ sở dữ liệu vào ComboBox
        private void LoadRoles()
        {
            var addUserController = new AddUserController();
            var roleNames = addUserController.GetRoleNames(); // Lấy danh sách vai trò từ controller

            foreach (var roleName in roleNames)
            {
                RoleComboBox.Items.Add(roleName); // Thêm vai trò vào ComboBox
            }

            if (RoleComboBox.Items.Count > 0)
            {
                RoleComboBox.SelectedIndex = 0; // Mặc định chọn vai trò đầu tiên
            }
        }

        private void SaveUserButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string fullName = FullNameTextBox.Text;
            string password = PasswordBox.Password;
            string email = EmailTextBox.Text;
            string roleName = RoleComboBox.SelectedItem?.ToString();

            // Kiểm tra nếu có trường nào bị để trống
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(fullName) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(roleName))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin người dùng.");
                return;
            }

            // Tạo đối tượng User mới
            var newUser = new User
            {
                UserName = username,
                FullName = fullName,
                Password = password,
                Email = email
            };

            // Thêm logic lưu người dùng (gọi AddUserController)
            var addUserController = new AddUserController();
            bool isAdded = addUserController.AddUser(newUser.UserName, newUser.FullName, newUser.Password, newUser.Email, roleName);

            if (isAdded)
            {
                // Thông báo thêm người dùng thành công
                MessageBox.Show("Người dùng đã được thêm thành công!");

                // Gửi thông báo về QuanLyNguoiDungView
                OnUserAdded?.Invoke();
            }
            else
            {
                // Thông báo nếu có lỗi xảy ra khi thêm người dùng
                MessageBox.Show("Có lỗi xảy ra khi thêm người dùng.");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra nếu UserControl trước đó tồn tại
            if (_previousUserControl != null)
            {
                // Ẩn AddUserView
                this.Visibility = Visibility.Collapsed;

                // Hiển thị lại UserControl trước đó
                var parent = this.Parent as Panel;
                if (parent != null)
                {
                    parent.Children.Clear(); // Xóa các UserControl hiện tại
                    parent.Children.Add(_previousUserControl); // Thêm UserControl trước đó vào lại
                }
            }
        }
    }
}
