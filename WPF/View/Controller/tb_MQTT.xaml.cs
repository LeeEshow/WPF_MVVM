using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF.ViewModel;

namespace WPF.View.Controller
{
    /// <summary>
    /// tb_MQTT.xaml 的互動邏輯
    /// </summary>
    public partial class tb_MQTT : UserControl
    {
        public tb_MQTT()
        {
            InitializeComponent();
            this.DataContext = WPF_MVVM.MQTT;
        }

        // 連線 / 斷開
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WPF_MVVM.MQTT.Connect();
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }
    }
}
