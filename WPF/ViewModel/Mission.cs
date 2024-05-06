using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ToolBox.ExtensionMethods;
using WPF.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace WPF.ViewModel
{
    public class Mission
    {
        public sealed class Manager : ViewModelBase
        {
            #region 屬性
            /// <summary>
            /// 狀態
            /// </summary>
            public string Operate
            {
                get => _Operate;
                set
                {
                    if (_Operate != value)
                    {
                        _Operate = value;
                        Operate_Image = "";
                        OnPropertyChanged();
                    }
                }
            }
            private string _Operate = "Start";
            /// <summary>
            /// 狀態圖示路徑
            /// </summary>
            public string Operate_Image
            {
                get
                {
                    if (Operate == "Stop")
                    {
                        return "pack://siteoforigin:,,,/Resources/stop.png";
                    }
                    else
                    {
                        return "pack://siteoforigin:,,,/Resources/play.png";
                    }
                }
                set
                {
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// 任務清單
            /// </summary>
            public BindingList<Loop> Tasks = new BindingList<Loop>();
            /// <summary>
            /// 任務數量
            /// </summary>
            public int Tasks_Count 
            {
                get => this.Tasks.Count;
            }

            public Loop SelectedItem
            {
                get => _SelectedItem;
                set
                {
                    _SelectedItem = value;
                    OnPropertyChanged();
                }
            }
            private Loop _SelectedItem;
            #endregion 屬性


            #region 行為
            /// <summary>
            /// 初始化宣告
            /// </summary>
            public Manager()
            {
                // 抓取 Settings Tasks 反序列
                JsonConverter[] converters = { new MissionConverter() };
                this.Tasks = JsonConvert.DeserializeObject<BindingList<Loop>>(Settings.Default.Tasks,
                    new JsonSerializerSettings() { Converters = converters });

                Tasks.ListChanged += SaveTaskList;
            }
            /// <summary>
            /// 資料變更，並儲存資料
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public void SaveTaskList(object sender, ListChangedEventArgs e)
            {
                OnPropertyChanged("Tasks_Count");
                Settings.Default.Tasks = this.Tasks.ToJsonString();
            }

            /// <summary>
            /// 全部啟動
            /// </summary>
            public void Start()
            {
                try
                {
                    Operate = "Stop";
                    foreach (var item in Tasks)
                    {
                        if (!item.IsRunning)
                        {
                            item.Run();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
                }
            }
            /// <summary>
            /// 全部終止
            /// </summary>
            public void Stop()
            {
                try
                {
                    Operate = "Start";
                    foreach (var item in Tasks)
                    {
                        if (item.IsRunning)
                        {
                            item.Terminate();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
                }
            }
            #endregion 行為


            #region 抽象類別 JSON反序列處理
            public class MissionConverter : JsonConverter
            {
                public override bool CanConvert(Type objectType)
                {
                    return (objectType == typeof(Loop));
                }

                public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
                {
                    JObject jo = JObject.Load(reader);
                    switch (jo["Class"].Value<string>())
                    {
                        case "WPF.ViewModel.MoveFile":
                            return jo.ToObject<MoveFile>(serializer);


                        default:
                            return jo.ToObject<Loop>(serializer);
                    }
                }

                public override bool CanWrite => false;

                public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
                {
                    throw new NotImplementedException();
                }
            }
            #endregion 抽象類別 JSON反序列處理
        }

        /// <summary>
        /// 抽象迴圈任務，執行程序未實作
        /// </summary>
        public abstract class Loop : ViewModelBase
        {
            #region 屬性
            /// <summary>
            /// 任務標題
            /// </summary>
            [Property("任務標題")]
            public string Titel
            {
                get => _Titel;
                set
                {
                    if (_Titel != value)
                    {
                        _Titel = value;
                        OnPropertyChanged();
                    }
                }
            }
            private string _Titel = $"{Guid.NewGuid()}";
            /// <summary>
            /// 起始時間
            /// </summary>
            [Property("起始時間")]
            public DateTime Start_Time
            {
                get => _Start_Time;
                set
                {
                    if (_Start_Time != value)
                    {
                        _Start_Time = value;
                        OnPropertyChanged();
                    }
                }
            }
            private DateTime _Start_Time = DateTime.Now.Clone();
            /// <summary>
            /// 程序循環時間，最小值 1 秒
            /// </summary>
            [Property("程序循環週期")]
            public TimeSpan Cycle_Time
            {
                get => _Cycle_Time;
                set
                {
                    if (_Cycle_Time != value)
                    {
                        _Cycle_Time = value;
                        OnPropertyChanged();
                    }
                }
            }
            private TimeSpan _Cycle_Time = TimeSpan.Parse("00:00:01");
            /// <summary>
            /// 執行次數
            /// </summary>
            [Property("執行次數")]
            [JsonIgnore]
            public uint Count
            {
                get => _Count;
                set
                {
                    _Count = value;
                    OnPropertyChanged();
                }
            }
            private uint _Count = 0;
            /// <summary>
            /// 執行狀態
            /// </summary>
            [Property("執行狀態", CanEdit = false)]
            [JsonIgnore]
            public bool IsRunning
            {
                get => _IsRunning;
                set
                {
                    if (_IsRunning != value)
                    {
                        _IsRunning = value;
                        OnPropertyChanged();
                        OnPropertyChanged("Operate_Image");
                    }
                }
            }
            private bool _IsRunning = false;
            /// <summary>
            /// 狀態圖示路徑
            /// </summary>
            [Property("狀態圖示路徑", CanEdit = false)]
            public string Operate_Image
            {
                get
                {
                    if (IsRunning)
                    {
                        return "pack://siteoforigin:,,,/Resources/stop.png";
                    }
                    else
                    {
                        return "pack://siteoforigin:,,,/Resources/play.png";
                    }
                }
                set
                {
                    OnPropertyChanged();
                }
            }

            [Property("物件類別", CanEdit = false)]
            public string Class => this.GetType().FullName;

            private Task _Task;
            private CancellationTokenSource _CancelSource;
            #endregion 屬性


            #region 行為
            /// <summary>
            /// 開始執行
            /// </summary>
            /// <returns></returns>
            public bool Run()
            {
                #region 管制
                if (_Task != null)
                {
                    MVVM.Show($"{this.Titel} 任務正在執行。");
                }
                if (this.Cycle_Time < TimeSpan.Parse("00:00:01"))
                {
                    this.Cycle_Time = TimeSpan.Parse("00:00:01");
                }
                #endregion 管制

                _CancelSource = new CancellationTokenSource();
                this._Task = new Task(() =>
                {
                    MVVM.Show("開始執行 >> " + this.Titel);
                    while (!_CancelSource.IsCancellationRequested)
                    {
                        try
                        {
                            Process();
                        }
                        catch (Exception ex)
                        {
                            MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
                        }

                        this.Count++;
                        SpinWait.SpinUntil(() => false, Cycle_Time);
                    }

                }, _CancelSource.Token);
                this._Task.Start();
                this.IsRunning = true;
                return true;
            }
            /// <summary>
            /// 終止
            /// </summary>
            public bool Terminate()
            {
                try
                {
                    if (_Task != null)
                    {
                        this._CancelSource.Cancel();
                        this._CancelSource.Dispose();
                        this._Task = null;
                        this.IsRunning = false;
                    }
                    else
                    {
                        this.IsRunning = false;
                    }
                    MVVM.Show($"{this.Titel} 任務已停止。");
                    return true;
                }
                catch (Exception ex)
                {
                    MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
                    return false;
                }
            }

            /// <summary>
            /// 抽象處理程序，繼承時實作處理
            /// </summary>
            public abstract void Process();
            #endregion 行為
        }
    }

    #region 自定義特性
    /// <summary>
    /// 屬性可否編輯
    /// </summary>
    public class PropertyAttribute : Attribute
    {
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 可否編輯
        /// </summary>
        public bool CanEdit { get; set; } = true;

        public PropertyAttribute(string Name, string Description = "", bool CanEdit = true)
        {
            this.Name = Name;
            this.Description = Description;
            this.CanEdit = CanEdit;
        }
    }
    #endregion 自定義特性

}
