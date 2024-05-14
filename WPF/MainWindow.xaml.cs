using MahApps.Metro.Controls;
using System;
using System.Deployment.Application;
using System.Windows;
using WPF.Properties;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using API;
using ToolBox.ExtensionMethods;
using TestCode;
using Component;
using System.Linq;
using Newtonsoft.Json;
using System.ComponentModel;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using WPF.ViewModel;
using WPF.View.Window;

namespace WPF
{
    /// <summary>
    /// 主視窗
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// 起始程序
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            #region 版本號
            try
            {
                this.Title += " - v" + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch
            {
                this.Title += " - 開發版本";
            }
            #endregion 版本號

            // 事件宣告
            Settings.Default.PropertyChanged += Default_PropertyChanged;
            // 把所有例外事件統一傳遞給 MVVM 處理
            Server.ServerException += MVVM.ExceptionEvent;
            ToolBox.Event.ToolException += MVVM.ExceptionEvent;
            API.Base.APIException += MVVM.ExceptionEvent;
        }

        #region 事件宣告
        private void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                _ = Task.Run(() =>
                  {
                      lock (Settings.Default)
                      {
                          Settings.Default.Save();
                      }
                  });
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }
        #endregion 事件宣告

        #region Windows 事件

        #region 工具列設定
        private System.Windows.Forms.NotifyIcon notifyIcon1 = new System.Windows.Forms.NotifyIcon();
        private void Window_StateChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.WindowState == WindowState.Minimized)
                {
                    this.Hide();
                    notifyIcon1.Icon = Properties.Resources.logo;
                    notifyIcon1.Text = this.Title;
                    notifyIcon1.Visible = true;

                    notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(notifyIcon1_MouseDoubleClick);
                }
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
                this.Close();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                this.Show();
                this.WindowState = WindowState.Normal;
                notifyIcon1.Visible = false;
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
                this.Close();
            }
        }

        #endregion 工具列設定

        // 程式開啟後所有要預先處理的事情都丟在這
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 啟動時自動執行
                if (MVVM.DBConfig.AutoTest)
                {
                    MVVM.DBConfig.TestConnect();
                }
                if (WPF_MVVM.MQTT.AutoConnect)
                {
                    WPF_MVVM.MQTT.Connect();
                }
                foreach (var mission in WPF_MVVM.Manager.Tasks)
                {
                    if (mission.AutoRun)
                    {
                        mission.Run();
                    }
                }
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            WPF_MVVM.Manager.Stop();

            // 關閉所有子視窗
            Window[] buffer = Application.Current.Windows.Cast<Window>().ToArray();
            foreach (var win in buffer)
            {
                if (win.Title != this.Title)
                {
                    win.Close();
                }
            }
        }
        #endregion Windows 事件


        // ---------------------------------------------------------------------------------------
        // Test

        private void btn_test1_Click(object sender, RoutedEventArgs e)
        {
            



        }



        private void btn_test2_Click(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
