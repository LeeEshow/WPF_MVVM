using API;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;

namespace WPF.ViewModel
{
    public class Error : ViewModelBase
    {
        #region 屬性
        /// <summary>
        /// 累計例外次數
        /// </summary>
        public int Count
        {
            get { return List.Count; }
            set
            {
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 閃爍燈號
        /// </summary>
        public bool Flashing
        {
            get { return _Flashing; }
            set
            {
                _Flashing = value;
                OnPropertyChanged();
            }
        }
        private bool _Flashing;
        /// <summary>
        /// 例外清單
        /// </summary>
        public List<Exception_Info> List = new List<Exception_Info>();
        /// <summary>
        /// 終止條件
        /// </summary>
        public uint Termination_Qty
        {
            get { return _Termination_Qty; }
            set
            {
                _Termination_Qty = value;
                OnPropertyChanged();
            }
        }
        private uint _Termination_Qty = 10;
        #endregion 屬性


        #region 行為
        public Error()
        {
            Base.APIException += Process_Exception;
            VM.AppException += Process_Exception;
        }

        private void Process_Exception(MethodBase method, Exception ex)
        {
            // 紀錄
            try
            {
                List.Add(new Exception_Info
                {
                    Log_Time = DateTime.Now,
                    Message = ex.Message,
                    Method_Name = method.Name,
                    Log = ex.StackTrace.Remove(0, ex.StackTrace.LastIndexOf("於"))
                });
            }
            catch { }
            // Show
            VM.Show(ex.Message);

            // 處理 SQL 連線例外
            if (ex is SqlException)
            {
                VM.DB_Status.IsConnect = false;
            }

            // 處理過多例外訊息
            if (List.Count > _Termination_Qty)
            {
                // 終止程序
                VM.Show("程式累計超過 10 次錯誤。自動終止運行，請重制右上角警報燈號。");
                VM.Manager.Terminating();
            }

            // 操作處理
            Count++;
            Flashing = true;
        }
        #endregion 行為

    }

    public class Exception_Info
    {
        public string Message { get; set; }

        public string Method_Name { get; set; }

        public string Log { get; set; }

        public DateTime Log_Time { get; set; }
    }

}
