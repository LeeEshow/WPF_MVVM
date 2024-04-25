using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPF.ViewModel;

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
            tb_Status.DataContext = VM.Manager;
            btn_Ex.DataContext = VM.Error;
            dgr_Exception.ItemsSource = VM.Error.List;
        }

        // 啟動終止操作
        private void Button_StartStop(object sender, RoutedEventArgs e)
        {
            if (VM.Manager.Status == Operate_Status.Start)
            {
                VM.Manager.Registe(new Mission());
                VM.Manager.Start();
            }
            else
            {
                VM.Manager.Terminating();
            }
        }

        // 重制警報
        private void Button_Error(object sender, RoutedEventArgs e)
        {
            VM.Error.List = new List<Exception_Info>();
            dgr_Exception.ItemsSource = null;
            dgr_Exception.ItemsSource = VM.Error.List;

            VM.Error.Count = 0;
            VM.Error.Flashing = false;
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
