using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Component;

namespace WPF
{
    /// <summary>
    /// Menu.xaml 的互動邏輯
    /// </summary>
    public partial class Menu : UserControl
    {
        public Menu()
        {
            InitializeComponent();
            mi_Mission_Manager.DataContext = MVVM.Manager;
        }

        // 子視窗 - 連線設定
        private void Connecting_Setting(object sender, RoutedEventArgs e)
        {
            Connection_Setting Setting = new Connection_Setting
            {
                Title = "設定後請測試連線",
            };
            Setting.ShowDialog();
        }

        // 子視窗 - FTP
        private void FTP_Setting(object sender, RoutedEventArgs e)
        {
            FTP ftp = new FTP();
            ftp.Show();
        }

        // 子視窗 - 任務管理
        private void Mission_Manager(object sender, RoutedEventArgs e)
        {
            Mission_Manager Manager = new Mission_Manager();
            Manager.Show();
        }
    }
}
