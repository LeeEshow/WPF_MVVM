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
using Newtonsoft.Json;
using System.Windows;

namespace WPF.ViewModel
{
    // 目前 ToolBox 專案使用.net Framework 4.7，C# 版本為 7.3
    // 因此無法使用 Interface + 預先實作的方法建立 MQTT 介面
    // 不得已只能改成 繼承 MQTT + 使用 INotifyPropertyChanged 介面
    // 如果是 C# 8 就可以這樣做 => MQTT : ViewModelBase, IMQTT.Client
    public class MQTT : Client, INotifyPropertyChanged
    {
        #region ViewModel 實作
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion ViewModel 實作

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
        /// 是否啟動程序時自動連線
        /// </summary>
        public bool AutoConnect
        {
            get => Settings.Default.MQTT_AutoConnect;
            set
            {
                Settings.Default.MQTT_AutoConnect = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 操作圖示
        /// </summary>
        public string Operate_Image
        {
            get => Operate_Image_;
            set
            {
                Operate_Image_ = value;
                OnPropertyChanged();
            }
        }
        private string Operate_Image_ = "/WPF;component/Image/link.png";
        /// <summary>
        /// 操作狀態
        /// </summary>
        public string Operate
        {
            get => Operate_;
            set
            {
                Operate_ = value;
                OnPropertyChanged();
            }
        }
        private string Operate_ = "連線";
        /// <summary>
        /// 連線狀態燈號
        /// </summary>
        public string Signal
        {
            get { return _Signal; }
            set
            {
                _Signal = value;
                OnPropertyChanged();
            }
        }
        private string _Signal = "Black";
        /// <summary>
        /// 訂閱頻道清單
        /// </summary>
        public BindingList<MQTT_Topic> Topics => Topics_;
        private BindingList<MQTT_Topic> Topics_ = new BindingList<MQTT_Topic>();
        /// <summary>
        /// 訊息清單
        /// </summary>
        public List<MQTT_Message> Messages = new List<MQTT_Message>();
        #endregion 屬性

        #region 行為
        /// <summary>
        /// 初始化
        /// </summary>
        public MQTT()
        {
            Topics_ = Settings.Default.MQTT_Topic.ToObject<BindingList<MQTT_Topic>>();
            Topics_.ListChanged += SaveConfig;
            this.StatusChanged += MQTT_StatusChanged;
            this.MessageReceived += MQTT_MessageReceived;
        }
        internal void SaveConfig(object sender, ListChangedEventArgs e)
        {
            Settings.Default.MQTT_Topic = this.Topics_.ToJsonString();
        }


        /// <summary>
        /// 訂閱頻道
        /// </summary>
        /// <param name="Topic"></param>
        async public override Task<bool> Subscribe(string Topic)
        {
            var data = Topics_.ToList().Find(x => x.Topic == Topic);
            if (data == null)
            {
                Topics_.Add(new MQTT_Topic
                {
                    ShowMessage = true,
                    Topic = Topic,
                    ToMainWindow = false
                });
            }

            if (IsConnected)
            {
                if (await base.Subscribe(Topic))
                {
                    MVVM.Show($"Subscribe Topic：{Topic}");
                }
                else
                {
                    data = Topics_.ToList().Find(x => x.Topic == Topic);
                    Application.Current?.Dispatcher.InvokeAsync(() =>
                    {
                        Topics_.Remove(data);
                    });
                }
            }
            return true;
        }
        /// <summary>
        /// 取消訂閱頻道
        /// </summary>
        /// <param name="Topic"></param>
        async public override Task<bool> Unsubscribe(string Topic)
        {
            var data = Topics_.ToList().Find(x => x.Topic == Topic);
            if (data != null)
            {
                if (IsConnected)
                {
                    MVVM.Show($"Unsubscribe Topic：{ Topic }");
                    await base.Unsubscribe(Topic);
                }

                Application.Current?.Dispatcher.InvokeAsync(() =>
                {
                    Topics_.Remove(data);
                });
            }
            return true;
        }

        /// <summary>
        /// 連線
        /// </summary>
        async public new void Connect()
        {
            if (!this.IsConnected)
            {
                if (!string.IsNullOrEmpty(this.Broker_IP) && !string.IsNullOrEmpty(this.ID))
                {
                    await base.Connect();
                }
            }
            else
            {
                await base.Disconnect();
            }
        }
        #endregion 行為

        #region Event
        async private void MQTT_StatusChanged(ToolBox.Common.MQTT.MQTT_ConnectStatus Status, string Message)
        {
            if (Status == ToolBox.Common.MQTT.MQTT_ConnectStatus.Connected)
            {
                Operate_Image = "/WPF;component/Image/link_off.png";
                Operate = "斷開";
                Signal = MVVM.Resources["Green"];

                MVVM.Show("MQTT is Connected");
                foreach (var item in WPF_MVVM.MQTT.Topics)
                {
                    await this.Subscribe(item.Topic);
                }
            }
            else
            {
                Operate_Image = "/WPF;component/Image/link.png";
                Operate = "連線";
                Signal = MVVM.Resources["Red"];

                MVVM.Show(string.IsNullOrEmpty(Message) ? $"MQTT：{Status.ToString()}" : $"MQTT is {Message}");
            }
            OnPropertyChanged("IsConnected");
        }

        private void MQTT_MessageReceived(ToolBox.Common.MQTT.MQTT_Message Message)
        {
            var topic = this.Topics.ToList().Find(x => x.Topic == Message.Topic);
            if (topic != null)
            {
                if (topic.ToMainWindow)
                {
                    MVVM.Show($"MQTT Message：{Message.Content}");
                }
            }

            Messages.Add(new MQTT_Message
            {
                DateTime = DateTime.Now,
                Message = Message
            });

            while (Messages.Count > 100)
            {
                Messages.RemoveAt(0);
            }
        }
        #endregion Event
    }

    #region Other Class
    public class MQTT_Topic : ViewModelBase
    {
        public bool ShowMessage 
        {
            get => ShowMessage_;
            set
            {
                ShowMessage_ = value;
                OnPropertyChanged();
            }
        }
        private bool ShowMessage_;

        public string Topic { get; set; }

        public bool ToMainWindow
        {
            get => ToMainWindow_;
            set
            {
                ToMainWindow_ = value;
                OnPropertyChanged();
            }
        }
        private bool ToMainWindow_;

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged();
            if (WPF_MVVM.MQTT != null)
            {
                WPF_MVVM.MQTT.SaveConfig(null, null);
            }
        }
    }

    public class MQTT_Message
    {
        public DateTime DateTime { get; set; }
        public ToolBox.Common.MQTT.MQTT_Message Message { get; set; }
    }
    #endregion Other Class

}
