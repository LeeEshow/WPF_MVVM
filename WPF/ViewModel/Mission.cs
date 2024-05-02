using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ToolBox.ExtensionMethods;

namespace WPF
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
            /// 清單
            /// </summary>
            public BindingList<Loop> Tasks = new BindingList<Loop>();

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
                Tasks.ListChanged += Tasks_ListChanged;
            }
            private void Tasks_ListChanged(object sender, ListChangedEventArgs e)
            {
                OnPropertyChanged("Tasks_Count");
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
            /// 執行狀態
            /// </summary>
            [PropEdit(false)]
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
            [PropEdit(false)]
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
            /// <summary>
            /// 執行次數
            /// </summary>
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
                    throw new Exception($"{this.Titel} 任務正在執行。");
                }
                if (this.Cycle_Time < TimeSpan.Parse("00:00:01"))
                {
                    this.Cycle_Time = TimeSpan.Parse("00:00:01");
                }
                #endregion 管制

                _CancelSource = new CancellationTokenSource();
                this._Task = new Task(() =>
                {
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
                        return true;
                    }
                    else
                    {
                        throw new Exception($"{this.Titel} 任務已停止。");
                    }
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
    public class PropEditAttribute : Attribute
    {
        /// <summary>
        /// 可否編輯
        /// </summary>
        public bool CanEdit { get; set; } = true;

        public PropEditAttribute(bool TF)
        {
            this.CanEdit = TF;
        }
    }
    #endregion 自定義特性

}
