using System;
using System.Linq;
using System.ComponentModel;
using System.Reflection;

namespace WPF.ViewModel
{
    public class Manager : ViewModelBase
    {
        #region 屬性
        /// <summary>
        /// 程序運行狀態
        /// </summary>
        public Operate_Status Status
        {
            get{ return _Status; }
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    Status_Image = "";
                    OnPropertyChanged();
                }
            }
        }
        private Operate_Status _Status = Operate_Status.Start;

        /// <summary>
        /// 狀態圖示路徑
        /// </summary>
        public string Status_Image
        {
            get 
            {
                if (_Tasks_Count != 0)
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
        /// 任務處理數量
        /// </summary>
        public int Tasks_Count 
        {
            get { return Tasks.Count; }
            set 
            {
                if (_Tasks_Count != value)
                {
                    _Tasks_Count = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _Tasks_Count;

        /// <summary>
        /// 管理處理程序
        /// </summary>
        public BindingList<Mission> Tasks = new BindingList<Mission>();
        #endregion 屬性


        #region 行為
        public Manager()
        {
            Tasks.ListChanged += Tasks_ListChanged;
        }

        #region 編輯
        /// <summary>
        /// 註冊任務
        /// </summary>
        /// <param name="Mission"></param>
        public bool Registe(Mission Mission)
        {
            try
            {
                lock (Tasks)
                {
                    if (!Tasks.Contains(Mission))
                    {
                        //Console.WriteLine($"Registe Title：{Mission.Titel} ");
                        Tasks.Add(Mission);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                VM.TriggerExceptionEvent(MethodBase.GetCurrentMethod(), ex);
                return false;
            }
        }

        private void Tasks_ListChanged(object sender, ListChangedEventArgs e)
        {
            this.Tasks_Count = Tasks.Count;
        }
        #endregion 編輯

        /// <summary>
        /// 啟動
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            try
            {
                if (Tasks.Count != 0 & this.Status == Operate_Status.Start)
                {
                    VM.Show("Start process.");
                    foreach (var item in Tasks)
                    {
                        if (!item.Task.IsCompleted)
                        {
                            //Console.WriteLine($"Start Title：{item.Titel} ");
                            item.Task.Start();
                        }
                    }
                    this.Status = Operate_Status.Stop;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                VM.TriggerExceptionEvent(MethodBase.GetCurrentMethod(), ex);
                return false;
            }
        }
        /// <summary>
        /// 終止
        /// </summary>
        /// <returns></returns>
        public bool Terminating()
        {
            try
            {
                if (this.Status == Operate_Status.Stop)
                {
                    foreach (var item in VM.Manager.Tasks.ToArray())
                    {
                        item.CancelSource.Cancel();
                        item.Task.Dispose();
                        //Console.WriteLine($"Terminating Title：{item.Titel} ");
                    }
                    Tasks.Clear();
                    VM.Show($"Terminating all tasks !!");
                    this.Status = Operate_Status.Start;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                VM.TriggerExceptionEvent(MethodBase.GetCurrentMethod(), ex);
                return false;
            }
        }

        #endregion 行為
    }

    public enum Operate_Status
    {
        /// <summary>
        /// 執行中
        /// </summary>
        Stop = 0,
        /// <summary>
        /// 等待執行
        /// </summary>
        Start = 1,
        /// <summary>
        /// 中止
        /// </summary>
        Terminating = 2,
    }

}
