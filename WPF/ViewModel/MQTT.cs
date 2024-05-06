using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ToolBox.Common;
using ToolBox.ExtensionMethods;
using WPF.Properties;
using Client = ToolBox.Common.MQTT.Client;

namespace WPF.ViewModel
{
    // 目前 ToolBox 專案使用.net Framework 4.7，C# 版本為 7.3
    // 因此無法使用 Interface + 預先實作的方法建立 MQTT 介面
    // 不得已只能改成 繼承 MQTT + 使用 INotifyPropertyChanged 介面
    // 如果是 C# 8 就可以這樣做 => MQTT : ViewModelBase, IMQTT.Client
    public class MQTT : Client, INotifyPropertyChanged
    {
        #region 屬性
        /// <summary>
        /// MQTT ID
        /// </summary>
        public override string ID
        {
            get => Settings.Default.MQTT_ID;
            set
            {
                Settings.Default.MQTT_ID = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Broker IP
        /// </summary>
        public override string Broker_IP
        {
            get => Settings.Default.MQTT_IP;
            set
            {
                Settings.Default.MQTT_IP = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 連線 Port
        /// </summary>
        public override int Port
        {
            get => Settings.Default.MQTT_Port;
            set
            {
                Settings.Default.MQTT_Port = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 是否保留訊息
        /// </summary>
        public override bool RetainMessage
        {
            get => Settings.Default.MQTT_RetainMessage;
            set
            {
                Settings.Default.MQTT_RetainMessage = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 訂閱頻道清單
        /// </summary>
        public BindingList<MQTT_Topic> Topics { get; set; } = new BindingList<MQTT_Topic>();
        private List<string> Topics_ = new List<string>();
        #endregion 屬性

        #region ViewModel 實作
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion ViewModel 實作

        #region 行為
        public MQTT()
        {
            Topics.ListChanged += Topics_ListChanged;
        }

        private void Topics_ListChanged(object sender, ListChangedEventArgs e)
        {
            // 處理頻道訂閱
            this.Unsubscribe(Topics_.ToArray());
            Topics_ = Topics.Select(x => x.Topic).ToList();
            this.Subscribe(Topics_.ToArray());
        }
        #endregion 行為
    }


    public class MQTT_Topic
    {
        public bool ShowMessage { get; set; }
        public string Topic { get; set; }
    }


}
