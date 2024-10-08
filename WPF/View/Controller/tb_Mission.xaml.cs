﻿using System.Windows;
using System.Windows.Controls;
using WPF.ViewModel;
using WPF.View.Window;

namespace WPF.View.Controller
{
    /// <summary>
    /// tb_Mission.xaml 的互動邏輯
    /// </summary>
    public partial class tb_Mission : UserControl
    {
        public tb_Mission()
        {
            InitializeComponent();
            this.DataContext = WPF_MVVM.Manager;
        }

        // 啟動終止操作
        private void Button_StartStop(object sender, RoutedEventArgs e)
        {
            if (WPF_MVVM.Manager.Operate == "Start")
            {
                WPF_MVVM.Manager.Start();
            }
            else
            {
                WPF_MVVM.Manager.Stop();
            }
        }

        // 開啟任務管理器
        private void Button_MissionManager(object sender, RoutedEventArgs e)
        {
            Mission_Manager Manager = new Mission_Manager();
            Manager.ShowOnly();
        }
    }
}
