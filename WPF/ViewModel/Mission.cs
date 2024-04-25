using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPF.ViewModel
{
    /// <summary>
    /// 執行任務
    /// </summary>
    public sealed class Mission : ViewModelBase
    {
        /// <summary>
        /// 任務標題
        /// </summary>
        public string Titel { get; set; } = $"{DateTime.Now.ToString("mm-ss.ff")}";
        /// <summary>
        /// 程序循環時間
        /// </summary>
        public TimeSpan Cycle_Time 
        {
            get { return _Cycle_Time; }
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
        /// 終止源
        /// </summary>
        public CancellationTokenSource CancelSource = new CancellationTokenSource();

        /// <summary>
        /// 任務
        /// </summary>
        public Task Task
        {
            get
            {
                return new Task(() =>
                {
                    while (!CancelSource.IsCancellationRequested)
                    {
                        try
                        {
                            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            List<int> list = null;
                            list.Add(12);
                        }
                        catch (Exception ex)
                        {
                            VM.TriggerExceptionEvent(MethodBase.GetCurrentMethod(), ex);
                        }
                        SpinWait.SpinUntil(() => false, Cycle_Time);
                    }
                }, CancelSource.Token);
            }
        }

    }
}
