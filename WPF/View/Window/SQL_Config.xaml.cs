using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace WPF.View.Window
{
    /// <summary>
    /// SQL_Config.xaml 的互動邏輯
    /// </summary>
    public partial class SQL_Config : MetroWindow
    {
        public SQL_Config()
        {
            InitializeComponent();
            this.DataContext = WPF_MVVM.MSSQLs.EditItem;
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!WPF_MVVM.MSSQLs.ToList().Select(x=>x.IP).ToList().Contains(WPF_MVVM.MSSQLs.EditItem.IP))
                {
                    WPF_MVVM.MSSQLs.Add(WPF_MVVM.MSSQLs.EditItem);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }

        private void DockPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Test_Click(null, new RoutedEventArgs());
            }
        }
    }
}
