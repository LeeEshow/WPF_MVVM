using MahApps.Metro.Controls;
using System.Linq;
using WPF.ViewModel;
using System.Windows;
using System.Windows.Input;
using System;
using System.Reflection;
using System.Collections.Generic;

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

            WPF_MVVM.MQTT.Topics.ListChanged += Topics_ListChanged;
        }

        // 重整清單
        private void Topics_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            try
            {
                gd_Topic.Items.Refresh();

                // 發布下拉選單重新更新
                var item = cbb_Topic.Text;
                List<string> list = WPF_MVVM.MQTT.Topics.ToList().Select(x => x.Topic).ToList();
                cbb_Topic.ItemsSource = list;
                cbb_Topic.Items.Refresh();
                cbb_Topic.SelectedIndex = list.FindIndex(x => x == item);
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }

        #region Topic 處理
        // 訂閱頻道
        private void Subscribe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_Subscribe.Text))
                {
                    var item = WPF_MVVM.MQTT.Topics.ToList().Find(x => x.Topic == txt_Subscribe.Text);
                    if (item == null)
                    {
                        WPF_MVVM.MQTT.Topics.Add(new MQTT_Topic
                        {
                            ShowMessage = true,
                            Topic = txt_Subscribe.Text
                        });
                    }
                }
                txt_Subscribe.Text = "";
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
        private void Unsubscribe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectItem != null)
                {
                    var item = WPF_MVVM.MQTT.Topics.ToList().Find(x => x.Topic == SelectItem.Topic);
                    WPF_MVVM.MQTT.Topics.Remove(item);
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
    }
}
