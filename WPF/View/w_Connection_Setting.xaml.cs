using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
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
using WPF.Properties;

namespace WPF
{
    /// <summary>
    /// Connection_Setting.xaml 的互動邏輯
    /// </summary>
    public partial class Connection_Setting : MetroWindow
    {
        public Connection_Setting()
        {
            InitializeComponent();
            gri_DB.DataContext = VM.DB_Status;
        }

        // 連線測試
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VM.DB_Status.TestConnect();
        }
        delegate void TextCallBack(object sender, string Text);
        void syncText(object sender, string Text)
        {
            if (!Dispatcher.CheckAccess())
            {
                TextCallBack myUIpdate = new TextCallBack(syncText);
                Dispatcher.Invoke(myUIpdate, sender, Text);
            }
            else
            {
                var obj =(TextBlock)sender;
                obj.Text = Text;
            }
        }


    }
}
