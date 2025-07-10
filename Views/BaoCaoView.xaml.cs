using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PlantManagement.Controllers;
using PlantManagement.Models;

namespace PlantManagement.Views
{
    public partial class BaoCaoView : UserControl
    {
        private BaoCaoController _baoCaoController;
        private Button _currentButton; // Biến lưu nút hiện tại đã được chọn

        public BaoCaoView()
        {
            InitializeComponent();
            _baoCaoController = new BaoCaoController();
            _currentButton = null; // Không có nút nào được chọn ban đầu
        }

        // Ẩn tất cả các giao diện
        private void HideAllViews()
        {
            UserDataGrid.Visibility = Visibility.Collapsed;
            LoginHistoryDataGrid.Visibility = Visibility.Collapsed;
            ImpactHistoryDataGrid.Visibility = Visibility.Collapsed;
            TongHopDataGrid.Visibility = Visibility.Collapsed;
            MessagePlaceholder.Visibility = Visibility.Collapsed;
        }

        // Xử lý sự kiện khi bấm vào một nút trong Expander
        private void ExpanderButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                // Thay đổi tiêu đề của Expander
                BaoCaoExpander.Header = clickedButton.Content;

                // Thu gọn Expander
                BaoCaoExpander.IsExpanded = false;

                // Cập nhật màu nền của nút
                SetButtonBackground(clickedButton);

                // Ẩn tất cả các DataGrid
                HideAllViews();

                // Hiển thị nội dung tương ứng với nút
                switch (clickedButton.Name)
                {
                    case "NguoiDungButton":
                        var userList = _baoCaoController.GetUserList();
                        UserDataGrid.ItemsSource = userList;
                        UserDataGrid.Visibility = Visibility.Visible;
                        break;
                    case "LichSuTruyCapButton":
                        var loginHistoryList = _baoCaoController.GetLoginHistoryWithFullName();
                        LoginHistoryDataGrid.ItemsSource = loginHistoryList;
                        LoginHistoryDataGrid.Visibility = Visibility.Visible;
                        break;
                    case "LichSuTacDongButton":
                        var impactHistoryTable = _baoCaoController.GetLoginHistoryData();

                        // Xóa dữ liệu cũ khỏi DataGrid
                        ImpactHistoryDataGrid.ItemsSource = null;

                        // Gán dữ liệu mới
                        ImpactHistoryDataGrid.ItemsSource = impactHistoryTable.DefaultView;
                        ImpactHistoryDataGrid.Visibility = Visibility.Visible;
                        break;

                    case "TongHopButton":
                        var tongHopData = _baoCaoController.GetTongHopData();
                        TongHopDataGrid.ItemsSource = tongHopData;
                        TongHopDataGrid.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        // Thay đổi màu nền của nút
        private void SetButtonBackground(Button clickedButton)
        {
            if (_currentButton != null)
            {
                _currentButton.Background = System.Windows.Media.Brushes.White; // Trở về màu gốc của nút trước
            }

            clickedButton.Background = System.Windows.Media.Brushes.LightGreen; // Đặt màu xanh cho nút hiện tại
            _currentButton = clickedButton; // Lưu lại nút hiện tại
        }

        // Hiển thị giao diện người dùng
        public void NguoiDung_Click(object sender, RoutedEventArgs e)
        {
            HideAllViews();
            SetButtonBackground(NguoiDungButton); // Thay đổi màu nền cho nút người dùng

            // Lấy danh sách người dùng từ controller
            List<User> userList = _baoCaoController.GetUserList();

            // Gán danh sách vào DataGrid
            UserDataGrid.ItemsSource = userList;
            UserDataGrid.Visibility = Visibility.Visible;
        }

        // Hiển thị giao diện lịch sử truy cập
        private void LichSuTruyCap_Click(object sender, RoutedEventArgs e)
        {
            HideAllViews();
            SetButtonBackground(LichSuTruyCapButton); // Thay đổi màu nền cho nút lịch sử truy cập

            // Lấy danh sách lịch sử truy cập từ controller (bao gồm FullName từ bảng User)
            List<LoginHistory> loginHistoryList = _baoCaoController.GetLoginHistoryWithFullName();

            // Gán danh sách vào DataGrid
            LoginHistoryDataGrid.ItemsSource = loginHistoryList; // Tự động tạo cột FullName
            LoginHistoryDataGrid.Visibility = Visibility.Visible;
        }

        // Hiển thị giao diện lịch sử tác động
       private void LichSuTacDong_Click(object sender, RoutedEventArgs e)
{
    HideAllViews();
    SetButtonBackground(LichSuTacDongButton); // Thay đổi màu nền cho nút lịch sử tác động

            // Lấy danh sách dữ liệu từ controller
            DataTable loginHistoryData = new DataTable();
               loginHistoryData = _baoCaoController.GetLoginHistoryData();

 

    // Gán DataTable vào ItemsSource của DataGrid
    ImpactHistoryDataGrid.ItemsSource = loginHistoryData.DefaultView;
    ImpactHistoryDataGrid.Visibility = Visibility.Visible;
}




        // Hiển thị giao diện tổng hợp
        // Hiển thị giao diện tổng hợp
        private void TongHop_Click(object sender, RoutedEventArgs e)
        {
            HideAllViews();
            SetButtonBackground(TongHopButton); // Thay đổi màu nền cho nút tổng hợp

            // Lấy dữ liệu tổng hợp từ controller
            List<TongHopData> tongHopData = _baoCaoController.GetTongHopData();

            // Gán danh sách vào DataGrid
            TongHopDataGrid.ItemsSource = tongHopData;
            TongHopDataGrid.Visibility = Visibility.Visible;
        }

    }
}
