using System.Data;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PlantManagement.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace PlantManagement.Views
{
    public partial class QuanLyNguoiDungView : UserControl
    {
        private UserInfoController _userController;
        private Button _currentButton; // Biến lưu nút hiện tại đã được chọn
        public QuanLyNguoiDungView()
        {
            InitializeComponent();
            _userController = new UserInfoController();
            _currentButton = null; // Không có nút nào được chọn ban đầu
            // Mặc định hiển thị trang Nhóm Người Dùng
            UserButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void ExpanderButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                // Thay đổi tiêu đề của Expander
                QuanLyNguoiDungExpander.Header = clickedButton.Content;

                // Thu gọn Expander
                QuanLyNguoiDungExpander.IsExpanded = false;

                // Cập nhật màu nền của nút
                ResetButtonBackground(clickedButton);

                // Ẩn tất cả các DataGrid
                HideAllDataGrids();

                // Hiển thị DataGrid tương ứng
                switch (clickedButton.Name)
                {
                    

                    case "UserButton":
                        var userList = _userController.GetUserList();
                        UserDataGrid.ItemsSource = userList;
                        UserDataGrid.Visibility = Visibility.Visible;

                        // Hiển thị nút Cập nhật,Xóa,Tìm kiếm và Thêm mới khi hiển thị bảng Người Dùng
                        UpdateDeleteUserPanel.Visibility = Visibility.Visible;
                        AddNewUserPanel.Visibility = Visibility.Visible;
                        SearchUserPanel.Visibility = Visibility.Visible;
                        break;

                    case "PermissionButton":
                        var permissionTable = _userController.GetPermissionData();
                        PermissionDataGrid.ItemsSource = null;
                        PermissionDataGrid.ItemsSource = permissionTable.DefaultView;
                        PermissionDataGrid.Visibility = Visibility.Visible;
                        break;

                    case "AccessHistoryButton":
                        var loginHistoryList = _userController.GetLoginHistoryWithFullName();
                        AccessHistoryDataGrid.ItemsSource = loginHistoryList;
                        AccessHistoryDataGrid.Visibility = Visibility.Visible;
                        SearchAccessHistoryPanel.Visibility = Visibility.Visible;
                        break;

                    case "ActionHistoryButton":
                        var impactHistoryTable = _userController.GetLoginHistoryData();
                        ActionHistoryDataGrid.ItemsSource = null;
                        ActionHistoryDataGrid.ItemsSource = impactHistoryTable.DefaultView;
                        ActionHistoryDataGrid.Visibility = Visibility.Visible;
                        SearchActionHistoryPanel.Visibility = Visibility.Visible;
                        break;
                }

                // Ẩn nút Cập nhật,Xóa,Tìm kiếm và Thêm mới nếu không phải bảng Người Dùng
                if (clickedButton.Name != "UserButton")
                {
                    UpdateDeleteUserPanel.Visibility = Visibility.Collapsed;
                    AddNewUserPanel.Visibility = Visibility.Collapsed;
                    SearchUserPanel.Visibility = Visibility.Collapsed;
                }
                if (clickedButton.Name != "AccessHistoryButton")
                {
                    SearchAccessHistoryPanel.Visibility = Visibility.Collapsed;
                }
                if (clickedButton.Name != "ActionHistoryButton")
                {
                    SearchActionHistoryPanel.Visibility = Visibility.Collapsed;
                }

            }
        }


        private void ResetButtonBackground(Button clickedButton)
        {
            if (_currentButton != null)
            {
                _currentButton.Background = System.Windows.Media.Brushes.White; // Trở về màu gốc của nút trước
            }

            clickedButton.Background = System.Windows.Media.Brushes.LightGreen; // Đặt màu xanh cho nút hiện tại
            _currentButton = clickedButton; // Lưu lại nút hiện tại
        }

        private void HideAllDataGrids()
        {
            // Ẩn tất cả các DataGrid
            
            UserDataGrid.Visibility = Visibility.Collapsed;
            PermissionDataGrid.Visibility = Visibility.Collapsed;
            AccessHistoryDataGrid.Visibility = Visibility.Collapsed;
            ActionHistoryDataGrid.Visibility = Visibility.Collapsed;
        }
        


        public void User_Click(object sender, RoutedEventArgs e)
        {
            HideAllDataGrids();
            ResetButtonBackground(UserButton); // Thay đổi màu nền cho nút người dùng

            // Lấy danh sách người dùng từ controller
            List<User> userList = _userController.GetUserList();

            // Gán danh sách vào DataGrid
            UserDataGrid.ItemsSource = userList;
            UserDataGrid.Visibility = Visibility.Visible;
        }
        // Hiển thị giao diện lịch sử truy cập
        private void AccessHistory_Click(object sender, RoutedEventArgs e)
        {
            HideAllDataGrids();
            ResetButtonBackground(AccessHistoryButton); // Thay đổi màu nền cho nút lịch sử truy cập

            // Lấy danh sách lịch sử truy cập từ controller (bao gồm FullName từ bảng User)
            List<LoginHistory> loginHistoryList = _userController.GetLoginHistoryWithFullName();

            // Gán danh sách vào DataGrid
            AccessHistoryDataGrid.ItemsSource = loginHistoryList; // Tự động tạo cột FullName
            AccessHistoryDataGrid.Visibility = Visibility.Visible;
        }
        private void SearchAccessHistoryBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_userController == null || string.IsNullOrWhiteSpace(SearchAccessHistoryTextBox.Text))
            {
                // Hiển thị toàn bộ danh sách nếu không có từ khóa
                AccessHistoryDataGrid.ItemsSource = _userController.GetLoginHistoryWithFullName();
                return;
            }

            // Lấy danh sách lịch sử truy cập từ controller
            var loginHistoryList = _userController.GetLoginHistoryWithFullName();

            // Lọc danh sách theo từ khóa
            string keyword = SearchAccessHistoryTextBox.Text.Trim().ToLower();
            var filteredHistory = loginHistoryList.Where(history =>
                (!string.IsNullOrEmpty(history.FullName) && history.FullName.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(history.IPAddress) && history.IPAddress.ToLower().Contains(keyword)) ||
                history.LoginTime.ToString("yyyy-MM-dd HH:mm:ss").Contains(keyword)).ToList();

            // Cập nhật DataGrid với danh sách đã lọc
            AccessHistoryDataGrid.ItemsSource = new ObservableCollection<LoginHistory>(filteredHistory);
        }

        private void SearchAccessHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            SearchAccessHistoryBox_TextChanged(sender, null);
        }

        // Hiển thị giao diện lịch sử tác động
        private void ActionHistory_Click(object sender, RoutedEventArgs e)
        {
            HideAllDataGrids();
            ResetButtonBackground(ActionHistoryButton); // Thay đổi màu nền cho nút lịch sử tác động

            // Lấy danh sách dữ liệu từ controller
            DataTable loginHistoryData = new DataTable();
            loginHistoryData = _userController.GetLoginHistoryData();

            // Gán DataTable vào ItemsSource của DataGrid
            ActionHistoryDataGrid.ItemsSource = loginHistoryData.DefaultView;
            ActionHistoryDataGrid.Visibility = Visibility.Visible;
        }
        private void SearchActionHistoryBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_userController == null || string.IsNullOrWhiteSpace(SearchActionHistoryTextBox.Text))
            {
                // Hiển thị toàn bộ danh sách nếu không có từ khóa
                ActionHistoryDataGrid.ItemsSource = _userController.GetLoginHistoryData().DefaultView;
                return;
            }

            // Lọc danh sách theo từ khóa
            var impactHistoryTable = _userController.GetLoginHistoryData();
            string keyword = SearchActionHistoryTextBox.Text.Trim().ToLower();

            var filteredRows = impactHistoryTable.AsEnumerable()
                .Where(row =>
                    row["UserName"].ToString().ToLower().Contains(keyword) ||
                    row["LoginCount"].ToString().Contains(keyword))
                .CopyToDataTable();

            // Cập nhật DataGrid với danh sách đã lọc
            ActionHistoryDataGrid.ItemsSource = filteredRows.DefaultView;
        }

        private void SearchActionHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            SearchActionHistoryBox_TextChanged(sender, null);
        }

        // Hiển thị giao diện Quản Lý Quyền
        private void Permission_Click(object sender, RoutedEventArgs e)
        {
            HideAllDataGrids();
            ResetButtonBackground(PermissionButton); // Thay đổi màu nền cho nút "Quản Lý Quyền"

            // Lấy dữ liệu từ controller
            var permissionTable = _userController.GetPermissionData();

            // Gán dữ liệu vào DataGrid
            PermissionDataGrid.ItemsSource = null; // Đảm bảo xóa dữ liệu cũ
            PermissionDataGrid.ItemsSource = permissionTable.DefaultView; // Gán dữ liệu mới
            PermissionDataGrid.Visibility = Visibility.Visible; // Hiển thị DataGrid
        }
        private void UpdateUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserDataGrid.SelectedItem is User selectedUser)
            {
                // Cập nhật thông tin từ dòng hiện tại
                bool isUpdated = _userController.UpdateUser(selectedUser);

                if (isUpdated)
                {
                    MessageBox.Show("Cập nhật người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Làm mới danh sách sau khi cập nhật
                    var userList = _userController.GetUserList();
                    UserDataGrid.ItemsSource = userList;
                }
                else
                {
                    MessageBox.Show("Cập nhật người dùng thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một người dùng để cập nhật.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserDataGrid.SelectedItem is User selectedUser)
            {
                // Hiển thị xác nhận
                var result = MessageBox.Show($"Bạn có chắc muốn xóa người dùng {selectedUser.UserName} không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Thực hiện xóa trong cơ sở dữ liệu
                    bool isDeleted = _userController.DeleteUser(selectedUser.ID);
                    if (isDeleted)
                    {
                        MessageBox.Show("Xóa người dùng thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Cập nhật lại danh sách người dùng
                        var userList = _userController.GetUserList();
                        UserDataGrid.ItemsSource = userList;
                    }
                    else
                    {
                        MessageBox.Show("Xóa người dùng thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một người dùng để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void AddNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            // Truyền 'this' vào AddUserView constructor
            var addUserView = new AddUserView(this);  // 'this' là UserControl hiện tại (QuanLyNguoiDungView)

            // Đăng ký sự kiện OnUserAdded
            addUserView.OnUserAdded += () =>
            {
                // Làm mới danh sách người dùng sau khi thêm thành công
                var userList = _userController.GetUserList();
                UserDataGrid.ItemsSource = userList;

                // Hiển thị bảng Người Dùng
                HideAllDataGrids();
                ResetButtonBackground(UserButton);
                UserDataGrid.Visibility = Visibility.Visible;

                // Ẩn AddUserView
                AddUserContentControl.Visibility = Visibility.Collapsed;
            };

            // Gán AddUserView vào ContentControl và hiển thị
            AddUserContentControl.Content = addUserView;
            AddUserContentControl.Visibility = Visibility.Visible;
        }



        private void SearchUserBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_userController == null || string.IsNullOrWhiteSpace(SearchUserTextBox.Text))
            {
                // Hiển thị toàn bộ danh sách nếu không có từ khóa
                UserDataGrid.ItemsSource = _userController.GetUserList();
                return;
            }

            // Lấy danh sách người dùng từ controller
            var userList = _userController.GetUserList();

            // Lọc danh sách theo từ khóa
            string keyword = SearchUserTextBox.Text.Trim().ToLower();
            var filteredUsers = userList.Where(user =>
                (!string.IsNullOrEmpty(user.UserName) && user.UserName.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(user.FullName) && user.FullName.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(user.Email) && user.Email.ToLower().Contains(keyword))).ToList();

            // Cập nhật DataGrid với danh sách đã lọc
            UserDataGrid.ItemsSource = new ObservableCollection<User>(filteredUsers);
        }

        private void SearchUserButton_Click(object sender, RoutedEventArgs e)
        {
            SearchUserBox_TextChanged(sender, null);
        }



    }
}