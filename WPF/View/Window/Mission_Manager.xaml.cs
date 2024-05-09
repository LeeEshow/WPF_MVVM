using MahApps.Metro.Controls;
using System;
using System.Reflection;
using System.Windows;
using WPF.ViewModel;
using ToolBox.ExtensionMethods;
using System.Runtime.InteropServices;

namespace WPF.View.Window
{
    /// <summary>
    /// Mission_Manager.xaml 的互動邏輯
    /// </summary>
    public partial class Mission_Manager : MetroWindow
    {
        public Mission_Manager()
        {
            InitializeComponent();
            this.DataContext = WPF_MVVM.Manager;
            dg_Data.ItemsSource = WPF_MVVM.Manager.Tasks;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mission.Loop mission = WPF_MVVM.Manager.SelectedItem;

                if (mission != null)
                {
                    if (mission.IsRunning == false)
                    {
                        mission.Run();
                    }
                    else
                    {
                        mission.Terminate();
                    }
                }
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }

        private void Add_Mission(object sender, RoutedEventArgs e)
        {
            WPF_MVVM.Manager.Tasks.Add(new MoveFile()
            {
                Titel = "Test Mission - " + WPF_MVVM.Manager.Tasks.Count,
                Cycle_Time = TimeSpan.Parse("00:00:01")
            });
        }

        private void Delete_Mission(object sender, RoutedEventArgs e)
        {
            var task = (MoveFile)WPF_MVVM.Manager.SelectedItem;
            if (task.IsRunning)
            {
                task.Terminate();
            }
            WPF_MVVM.Manager.Tasks.Remove(task);
        }

        private void Edit_Mission(object sender, RoutedEventArgs e)
        {
            Mission_Manager_Setting setting = new Mission_Manager_Setting();
            setting.Mission = WPF_MVVM.Manager.SelectedItem;
            setting.ShowDialog();
        }
    }
}