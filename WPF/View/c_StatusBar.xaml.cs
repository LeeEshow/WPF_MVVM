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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF.Properties;

namespace WPF
{
    /// <summary>
    /// StatusBar.xaml 的互動邏輯
    /// </summary>
    public partial class StatusBar : UserControl
    {
        public StatusBar()
        {
            InitializeComponent();
            gri_DB.DataContext = VM.DB_Status;
            gri_Tasks.DataContext = VM.Manager;
        }
    }
}
