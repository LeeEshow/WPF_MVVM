using MahApps.Metro.Controls;
using System.Linq;
using WPF.ViewModel;
using System.Windows;
using System.Windows.Input;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Data;

namespace WPF.View.Window
{
    /// <summary>
    /// MQTT.xaml 的互動邏輯
    /// </summary>
    public partial class MQTT_Window : MetroWindow
    {
        public MQTT_Window()
        {
            InitializeComponent();
            this.DataContext = WPF_MVVM.MQTT;
            gd_Topic.DataContext = this;
            gd_Topic.ItemsSource = WPF_MVVM.MQTT.Topics;

            WPF_MVVM.MQTT.MessageReceived += MQTT_MessageReceived;
            WPF_MVVM.MQTT.StatusChanged += MQTT_StatusChanged;
            WPF_MVVM.MQTT.Topics.ListChanged += Topics_ListChanged;

            Topics_ListChanged(null , null);
        }

        // 預先載入
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in WPF_MVVM.MQTT.Messages)
            {
                ShowMessage(item.DateTime, item.Message);
            }
        }

        #region List/Text Changed
        private delegate void ListChangedCallBack(object sender, ListChangedEventArgs e);
        private void Topics_ListChanged(object sender, ListChangedEventArgs e)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    ListChangedCallBack myUIpdate = new ListChangedCallBack(Topics_ListChanged);
                    Dispatcher.Invoke(myUIpdate, sender, e);
                }
                else
                {
                    // 發布下拉選單重新更新
                    if (WPF_MVVM.MQTT.IsConnected)
                    {
                        var item = cbb_Topic.Text;
                        List<string> list = WPF_MVVM.MQTT.Topics.ToList().Select(x => x.Topic).ToList();
                        cbb_Topic.ItemsSource = list;
                        cbb_Topic.Items.Refresh();
                        cbb_Topic.SelectedIndex = list.FindIndex(x => x == item);
                    }
                    else
                    {
                        cbb_Topic.ItemsSource = null;
                        cbb_Topic.Items.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }

        private void RichTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                rtb_msage.ScrollToEnd();
                if (txt_MQTTMessage != null)
                {
                    while (txt_MQTTMessage.Inlines.Count > 200)
                    {
                        txt_MQTTMessage.Inlines.Remove(txt_MQTTMessage.Inlines.FirstInline);
                    }
                }
            }
            catch { }
        }
        #endregion List/Text Changed

        #region Topic 處理
        // 訂閱頻道
        private void Subscribe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_Subscribe.Text))
                {
                    if (WPF_MVVM.MQTT.Topics.ToList().Find(x => x.Topic == txt_Subscribe.Text) == null)
                    {
                        _ = WPF_MVVM.MQTT.Subscribe(txt_Subscribe.Text);
                    }
                    txt_Subscribe.Text = "";
                }
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }
        private void txt_Subscribe_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Subscribe_Click(null, new RoutedEventArgs());
            }
        }

        // 取消訂閱
        public MQTT_Topic SelectItem { get; set; }
        async private void Unsubscribe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectItem != null)
                {
                    string str = SelectItem.Topic;
                    _ = await WPF_MVVM.MQTT.Unsubscribe(str);
                }
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }
        #endregion Topic 處理

        #region 發布訊息
        private void Publish_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_Publish.Text) & cbb_Topic.SelectedIndex != -1)
                {
                    WPF_MVVM.MQTT.Publish(cbb_Topic.Text, txt_Publish.Text);
                }
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }
        #endregion 發布訊息

        #region MQTT 事件
        private void MQTT_MessageReceived(ToolBox.Common.MQTT.MQTT_Message Message)
        {
            ShowMessage(DateTime.Now, Message);
        }
        private void MQTT_StatusChanged(ToolBox.Common.MQTT.MQTT_ConnectStatus Status, string Message)
        {
            try
            {
                Topics_ListChanged(null, null);
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }

        // 委派畫面顯示 MQTT 訊息
        private delegate void MQTTMessageCallBack(DateTime DateTime, ToolBox.Common.MQTT.MQTT_Message Message);
        private void ShowMessage(DateTime DateTime, ToolBox.Common.MQTT.MQTT_Message Message)
        {
            if (!Dispatcher.CheckAccess())
            {
                MQTTMessageCallBack myUIpdate = new MQTTMessageCallBack(ShowMessage);
                Dispatcher.Invoke(myUIpdate, DateTime, Message);
            }
            else
            {
                #region 訊息前處理
                var topic = WPF_MVVM.MQTT.Topics.ToList().Find(x => x.Topic == Message.Topic);
                if (topic != null)
                {
                    if (!topic.ShowMessage)
                    {
                        return;
                    }
                }
                #endregion 訊息前處理

                #region 顯示訊息
                MQTT_Message message = new MQTT_Message
                {
                    DateTime = DateTime,
                    Message = Message
                };

                Run Newrun = new Run(Environment.NewLine + $"【{message.DateTime:MM/dd HH:mm:ss}】 Topic：{Message.Topic}");
                Newrun.FontSize = 13;
                txt_MQTTMessage.Inlines.Add(Newrun);

                Newrun = new Run(Environment.NewLine + $"{Message.Content}" + Environment.NewLine);
                Newrun.FontSize = 13;
                Newrun.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(MVVM.Resources["White"]);
                txt_MQTTMessage.Inlines.Add(Newrun);
                #endregion 顯示訊息
            }
        }
        #endregion MQTT 事件

        // 連線/離線處理
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WPF_MVVM.MQTT.Connect();
                if (WPF_MVVM.MQTT.Topics.ToList().Find(x => x.Topic == WPF_MVVM.MQTT.ID) == null)
                {
                    _ = WPF_MVVM.MQTT.Subscribe(WPF_MVVM.MQTT.ID);
                }
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }
    }


    public class BoolToOppositeBoolConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
