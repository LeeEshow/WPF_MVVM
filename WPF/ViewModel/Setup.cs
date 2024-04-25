using System;
using System.Reflection;
using System.Threading.Tasks;
using WPF.Properties;
using API;

namespace WPF.ViewModel
{
    public class DB_Status : ViewModelBase
    {
        #region 屬性
        /// <summary>
        /// SQL Server IP
        /// </summary>
        public string IP 
        {
            get => Settings.Default.DB_IP;
            set 
            { 
                Settings.Default.DB_IP = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// DB Connect ID
        /// </summary>
        public string ID
        {
            get => Settings.Default.DB_ID;
            set
            {
                Settings.Default.DB_ID = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// DB Connect Password
        /// </summary>
        public string Password
        {
            get => Settings.Default.DB_Password;
            set
            {
                Settings.Default.DB_Password = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Def Schema
        /// </summary>
        public string Schema
        {
            get => Settings.Default.Schema;
            set
            {
                Settings.Default.Schema = value;
                OnPropertyChanged();
            }
        }

        private bool _IsConnect;
        /// <summary>
        /// 是否已連線
        /// </summary>
        public bool IsConnect 
        {
            get { return _IsConnect; }
            set 
            {
                _IsConnect = value;
                Signal = value ? "Green" : "Red";
                OnPropertyChanged();
            }
        }

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

        public string Connect_Result
        {
            get { return _Connect_Result; }
            set 
            {
                _Connect_Result = value;
                OnPropertyChanged();
            }
        }
        private string _Connect_Result;

        #endregion 屬性


        #region 行為
        private bool IsTesting = false;
        /// <summary>
        /// 連線測試
        /// </summary>
        public void TestConnect()
        {
            try
            {
                _ = Task.Run(() =>
                {
                    this.Signal = "Black";
                    this.Connect_Result = "Testing";
                    if (!VM.DB_Status.IsTesting)
                    {
                        VM.DB_Status.IsTesting = true;
                        Server.MSSQL.Setting(Settings.Default.DB_IP,
                                      Settings.Default.DB_ID,
                                      Settings.Default.DB_Password,
                                      Settings.Default.Schema);

                        if (Server.MSSQL.IsConnected())
                        {
                            this.IsConnect = true;
                            this.Connect_Result = "Successful";
                        }
                        else
                        {
                            this.IsConnect = false;
                            this.Connect_Result = "Failure";
                        }
                        this.IsTesting = false;
                    }
                });
            }
            catch (Exception ex)
            {
                VM.TriggerExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }

        #endregion 行為
    }

}
