using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using WPF.ViewModel;

namespace WPF
{
    /// <summary>
    /// 作為所有 ViewModel 的基底，提供屬性變化時告知 UI 刷新的方法【OnPropertyChanged】。
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    // --------------------------------------------------------------------------------
    // 靜態宣告所有介面【共用】物件，像跨界面傳遞值或全域變數的意思。

    /// <summary>
    /// ViewModel
    /// </summary>
    public static class VM
    {
        // ViewModel 方式傳遞 
        #region ViewModel
        /// <summary>
        /// 管理處理程序
        /// </summary>
        public static Manager Manager = new Manager();

        /// <summary>
        /// 資料庫連線狀態
        /// </summary>
        public static DB_Status DB_Status = new DB_Status();

        /// <summary>
        /// 異常通報
        /// </summary>
        public static Error Error = new Error();
        #endregion ViewModel


        // 無法用 ViewModel 方式改用事件(event)傳遞
        #region Event

        #region Show Message
        /// <summary>
        /// 委派事件
        /// </summary>
        /// <param name="ex">例外內容</param>
        public delegate void MessageShow(string Message);
        /// <summary>
        /// MessageShow
        /// </summary>
        public static event MessageShow ShowEvent;
        /// <summary>
        /// Show
        /// </summary>
        /// <param name="ex"></param>
        public static void Show(string Message)
        {
            ShowEvent?.Invoke(Message);
        }
        #endregion Show Message

        #region Method Exception
        /// <summary>
        /// 委派事件
        /// </summary>
        /// <param name="ex">例外內容</param>
        public delegate void MethodException(MethodBase method, Exception ex);
        /// <summary>
        /// 例外事件。
        /// </summary>
        public static event MethodException AppException;
        /// <summary>
        /// 開專案中發生例外狀況必須觸發該事件。
        /// </summary>
        /// <param name="ex"></param>
        public static void TriggerExceptionEvent(MethodBase method, Exception ex)
        {
            AppException?.Invoke(method, ex);
        }
        #endregion Method Exception

        #endregion Event
    }
}
