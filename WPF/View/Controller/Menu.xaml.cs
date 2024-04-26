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

    }
}
