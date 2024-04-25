using MahApps.Metro.Controls;
using System;
using System.Deployment.Application;
using System.Windows;
using WPF.Properties;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Threading;
using WPF.ViewModel;
using System.Collections.Generic;
using System.Reflection;
using API;
using Eshow.ExtensionMethods;
using TestCode;

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
                tb_Version.Text = "v" + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch
            {
                tb_Version.Text = "開發環境";
            }
            #endregion 版本號

            // 事件宣告
            Settings.Default.PropertyChanged += Default_PropertyChanged;
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
                VM.TriggerExceptionEvent(MethodBase.GetCurrentMethod(), ex);
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
                VM.TriggerExceptionEvent(MethodBase.GetCurrentMethod(), ex);
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
                VM.TriggerExceptionEvent(MethodBase.GetCurrentMethod(), ex);
                this.Close();
            }
        }

        #endregion 工具列設定

        // 程式開啟後所有要預先處理的事情都丟在這
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // DB 連線測試
                VM.DB_Status.TestConnect();

            }
            catch (Exception ex)
            {
                VM.TriggerExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }
        #endregion Windows 事件


        // ---------------------------------------------------------------------------------------
        // Test
        private void btn_test1_Click(object sender, RoutedEventArgs e)
        {
            List<Customer_Code> List = new List<Customer_Code>();

            for (int x = 2000; x < 2999; x++)
            {
                List.Add(new Customer_Code
                {
                    Case = "NAL",
                    Customer = "Test",
                    Remark = "2024-04-24 Test 2",
                    Codes = new List<Serial_Code>
                    {
                        new Serial_Code { Key = "Unique Reference Number", Value = "A0425" + x.ToString("X").PadLeft(5, '0') },
                        new Serial_Code { Key = "15-Digit Digital Code", Value = "B0425" + x.ToString("X").PadLeft(5, '0') }
                    }
                });
            }

            var TF = new Customer_Code().BulkInsert(List);

            VM.Show(TF.ToString());
        }





        private void btn_test2_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
