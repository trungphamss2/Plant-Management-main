using System.Windows;
using System.Windows.Controls;
using PlantManagement.Models;
using PlantManagement.Controllers;
using System.Collections.Generic;

namespace PlantManagement.Views
{
    public partial class QuanLyThongTinTaiKhoanView : UserControl
    {
        private User _currentUser;
        private QuanLyThongTinTaiKhoanController _controller;

        public QuanLyThongTinTaiKhoanView(User user)
        {
            InitializeComponent();
            _currentUser = user;
            _controller = new QuanLyThongTinTaiKhoanController();
            DisplayUserInfo(user);
            LoadRoles();
        }

        // Hiển thị thông tin người dùng
        private void DisplayUserInfo(User user)
        {
            FullNameTextBox.Text = user.FullName;
            UserNameTextBlock.Text = user.UserName;
            EmailTextBox.Text = user.Email;
            CreatedAtTextBlock.Text = user.CreatedAt.ToString("dd/MM/yyyy");

            // Hiển thị RoleName vào ComboBox
            string roleName = _controller.GetRoleNameById(user.ID_Role); // Lấy RoleName từ RoleID
            RoleComboBox.SelectedValue = roleName;  // Truyền RoleName vào ComboBox

            // Kiểm tra RoleID để quyết định có thể sửa RoleName không
            if (user.ID_Role == 2) // Nếu là User (ID_Role == 2), không cho sửa Role
            {
                RoleComboBox.IsEnabled = false;  // Disable ComboBox (không thể sửa)
            }
            else if (user.ID_Role == 1) // Nếu là Admin (ID_Role == 1), có thể sửa Role
            {
                RoleComboBox.IsEnabled = true;  // Enable ComboBox (có thể sửa)
            }
        }

        // Tải danh sách các vai trò từ cơ sở dữ liệu vào ComboBox
        private void LoadRoles()
        {
            List<string> roleList = _controller.GetRoleNames();
            RoleComboBox.ItemsSource = roleList;
        }

        // Khi người dùng nhấn nút Lưu
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox.Text;
            string email = EmailTextBox.Text;
            string roleName = (string)RoleComboBox.SelectedValue;  // Lấy RoleName của vai trò được chọn

            // Lấy ID của Role từ RoleName
            int roleId = _controller.GetIdRoleByName(roleName);  // Lấy ID của Role từ RoleName

            // Cập nhật thông tin người dùng thông qua controller
            _currentUser.FullName = fullName;
            _currentUser.Email = email;
            _currentUser.ID_Role = roleId;  // Cập nhật ID_Role của người dùng

            // Truyền cả roleName khi gọi UpdateUserInfo
            bool updateSuccessful = _controller.UpdateUserInfo(_currentUser, roleName); // Thêm roleName ở đây

            if (updateSuccessful)
            {
                MessageBox.Show("Thông tin đã được lưu thành công.");
            }
            else
            {
                MessageBox.Show("Đã có lỗi xảy ra khi lưu thông tin.");
            }
        }

        // Khi người dùng nhấn nút Đổi mật khẩu
        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            // Ẩn phần thông tin tài khoản và nút đổi mật khẩu
            AccountInfoPanel.Visibility = Visibility.Collapsed;
            ForgotPasswordButton.Visibility = Visibility.Collapsed;

            // Hiển thị ResetPasswordView
            ResetPasswordView resetPasswordView = new ResetPasswordView(_currentUser);
            ResetPasswordContent.Content = resetPasswordView;  // Chuyển ResetPasswordView vào ContentControl
            ResetPasswordContent.Visibility = Visibility.Visible; // Hiển thị ResetPasswordView
        }
    }
}

