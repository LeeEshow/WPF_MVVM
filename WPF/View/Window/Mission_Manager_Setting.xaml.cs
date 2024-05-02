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

namespace WPF
{
    /// <summary>
    /// Mission_Manager_Setting.xaml 的互動邏輯
    /// </summary>
    public partial class Mission_Manager_Setting : MetroWindow
    {
        public Mission_Manager_Setting()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public Mission.Loop Mission { get; set; }
        public KeyValue Item { get; set; }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                List<KeyValue> list = new List<KeyValue>();
                if (Mission != null)
                {
                    foreach (var prop in Mission.GetType().GetProperties())
                    {
                        list.Add(new KeyValue
                        {
                            Key = prop.Name,
                            Value = prop.GetValue(Mission).ToString(),
                            // 依照屬性特性值 >> 決定可否提供編輯
                            CanEdit = prop.GetCustomAttribute<PropEditAttribute>() != null ?
                                prop.GetCustomAttribute<PropEditAttribute>().CanEdit : true
                        });
                    }
                }
                list.Sort((x, y) => x.Key.CompareTo(y.Key));
                dg_Data.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Item != null)
                {

                    PropertyInfo prop = WPF_MVVM.Manager.SelectedItem.GetType().
                        GetProperty(Item.Key);

                    TextBox txt = (TextBox)sender;

                    switch (prop.PropertyType.Name)
                    {
                        case "TimeSpan":
                            if(TimeSpan.TryParse(txt.Text, out TimeSpan time))
                            prop.SetValue(
                                    WPF_MVVM.Manager.SelectedItem,
                                    time,
                                    null);
                            break;

                        case "DateTime":
                            if (DateTime.TryParse(txt.Text, out DateTime date))
                            {
                                prop.SetValue(
                                    WPF_MVVM.Manager.SelectedItem,
                                    date,
                                    null);
                            }
                            break;

                        default:
                            prop.SetValue(
                                    WPF_MVVM.Manager.SelectedItem,
                                    Convert.ChangeType(txt.Text, prop.PropertyType),
                                    null);
                            break;
                    }
                }
            }
            catch (Exception ex)
          {
               
            }
        }
    }

    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool CanEdit { get; set; }
    }
}
