﻿using MahApps.Metro.Controls;
using System;
using System.Reflection;
using System.Windows;
using WPF.Processor;
using ToolBox.ExtensionMethods;
using System.Runtime.InteropServices;

namespace WPF
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
            WPF_MVVM.Manager.Tasks.ListChanged += Tasks_ListChanged;
        }

        private void Tasks_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            try
            {
                dg_Data.Items.Refresh();
            }
            catch { }
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
                Path = "123456",
                Titel = "Test Loop - " + WPF_MVVM.Manager.Tasks.Count,
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

namespace WPF.Processor
{
    public class MoveFile : Mission.Loop
    {
        public string Path { get; set; }

        public override void Process()
        {
            MVVM.Show(this.Titel + " >>>> Cycle " + this.Count);
        }
    }




}
