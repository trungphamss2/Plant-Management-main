using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using PlantManagement.Controllers;

namespace PlantManagement.Views
{
    public partial class DonViHanhChinhView : UserControl
    {
        private readonly DonViHanhChinhController _controller;

        private bool isSlideAnimationDone = false; // Biến flag để theo dõi hiệu ứng trượt


        public DonViHanhChinhView()
        {
            InitializeComponent();
            _controller = new DonViHanhChinhController();
            var slideAnimation = new DoubleAnimation
            {
                From = 0,
                To = 200, // Chiều cao cần trượt ra
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            HuyenStackPanel.Visibility = Visibility.Visible;
            HuyenStackPanel.BeginAnimation(HeightProperty, slideAnimation);

            LoadHuyenData();
        }

        // Khi bấm nút "Đơn Vị Hành Chính Huyện"
        private void HuyenButton_Click(object sender, RoutedEventArgs e)
        {
            // Tạo hiệu ứng trượt (Slide)
            
        }

        // Tải dữ liệu huyện (cấp 1)
        private void LoadHuyenData()
        {
            try
            {
                // Lấy danh sách các cấp từ controller (danh sách cấp hành chính)
                DataTable capTable = _controller.GetDanhMucCap();
                foreach (DataRow row in capTable.Rows)
                {
                    if (row["TenCap"].ToString() == "Huyện")  // Lọc cấp "Huyện"
                    {
                        int capId = Convert.ToInt32(row["ID"]);
                        DataTable huyenTable = _controller.GetDanhMucDonViHanhChinh(capId); // Lấy danh sách huyện
                        HuyenListBox.ItemsSource = huyenTable.DefaultView;  // Đặt nguồn dữ liệu cho HuyệnListBox
                        HuyenListBox.DisplayMemberPath = "TenDVHC"; // Hiển thị tên huyện
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu huyện: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Khi người dùng chọn một huyện
        private void HuyenListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HuyenListBox.SelectedItem != null)
            {
                try
                {
                    // Lấy ID của huyện đã chọn
                    var selectedHuyen = (DataRowView)HuyenListBox.SelectedItem;
                    int selectedHuyenId = Convert.ToInt32(selectedHuyen["ID"]);

                    // Tải danh sách xã theo huyện đã chọn
                    LoadXaData(selectedHuyenId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lấy thông tin huyện: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        // Tải danh sách xã theo huyện đã chọn
        private void LoadXaData(int huyenId)
        {
            try
            {
                // Lấy danh sách xã theo ID huyện
                DataTable xaTable = _controller.GetDanhMucXaByHuyen(huyenId);
                XaListBox.ItemsSource = xaTable.DefaultView;
                XaListBox.DisplayMemberPath = "TenDVHC"; // Hiển thị tên xã

                // Tạo hiệu ứng trượt cho danh sách xã
                var slideAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 200, // Chiều cao cần trượt ra
                    Duration = new Duration(TimeSpan.FromSeconds(0.5))
                };
                XaStackPanel.Visibility = Visibility.Visible; // Đảm bảo StackPanel là Visible
                XaStackPanel.BeginAnimation(HeightProperty, slideAnimation);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách xã: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        // Tìm kiếm
        // Xử lý tìm kiếm
        // Xử lý tìm kiếm huyện và xã
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string keyword = SearchTextBox.Text.Trim();

                if (!string.IsNullOrEmpty(keyword))
                {
                    DataTable searchResults = _controller.SearchDonViHanhChinh(keyword);

                    if (searchResults.Rows.Count > 0)
                    {
                        // Phân loại kết quả tìm kiếm
                        var huyenRows = searchResults.Select("TenCap = 'Huyện'");
                        var xaRows = searchResults.Select("TenCap = 'Xã'");

                        if (huyenRows.Length > 0)
                        {
                            HuyenListBox.ItemsSource = huyenRows.CopyToDataTable().DefaultView;
                            HuyenListBox.DisplayMemberPath = "TenDVHC";
                            HuyenStackPanel.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            HuyenListBox.ItemsSource = null;
                            HuyenStackPanel.Visibility = Visibility.Collapsed;
                        }

                        if (xaRows.Length > 0)
                        {
                            XaListBox.ItemsSource = xaRows.CopyToDataTable().DefaultView;
                            XaListBox.DisplayMemberPath = "TenDVHC";
                            XaStackPanel.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            XaListBox.ItemsSource = null;
                            XaStackPanel.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        // Thông báo không tìm thấy kết quả
                        MessageBox.Show("Không có đơn vị hành chính nào khớp với từ khóa tìm kiếm.", "Kết quả tìm kiếm", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadHuyenData(); // Tải lại danh sách huyện
                        XaListBox.ItemsSource = null;
                        XaStackPanel.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    LoadHuyenData();
                    XaListBox.ItemsSource = null;
                    XaStackPanel.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        // Khi TextBox tìm kiếm nhận được focus
        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            WatermarkText.Visibility = Visibility.Collapsed;
        }

        // Khi TextBox tìm kiếm mất focus
        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
            {
                WatermarkText.Visibility = Visibility.Visible;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Tải lại danh sách huyện đầy đủ
                LoadHuyenData();

                // Ẩn danh sách xã
                XaStackPanel.Visibility = Visibility.Collapsed;
                XaListBox.ItemsSource = null;

                // Xóa nội dung tìm kiếm
                SearchTextBox.Text = string.Empty;
                WatermarkText.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi quay lại danh sách huyện: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



    }
}
