using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF.ViewModel;

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
                if (!WPF_MVVM.MSSQLs.ToList().Contains(WPF_MVVM.MSSQLs.EditItem))
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
