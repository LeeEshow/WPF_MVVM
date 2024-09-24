using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using API;
using Newtonsoft.Json;
using WPF.Properties;
using System.Windows;
using System.Runtime.CompilerServices;

namespace WPF.ViewModel
{
    #region MSSQL 連線
    public class MSSQL_List : BindingList<MSSQL>, INotifyPropertyChanged
    {
        #region ViewModel 實作
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion ViewModel 實作

        #region 屬性
        public MSSQL EditItem { get; set; }

        public Visibility IsLoading
        {
            get => IsLoading_;
            set
            {
                IsLoading_ = value;
                OnPropertyChanged();
            }
        }
        private Visibility IsLoading_ = Visibility.Collapsed;
        #endregion 屬性

        #region 行為
        public MSSQL_List()
        {
            foreach (var item in Settings.Default.MSSQL_List.ToObject<List<MSSQL>>())
            {
                this.Add(item);
            }
            this.ListChanged += MSSQL_List_ListChanged;
        }

        private void MSSQL_List_ListChanged(object sender, ListChangedEventArgs e)
        {
            Settings.Default.MSSQL_List = this.ToJsonString();

            foreach (var item in WPF_MVVM.TSQLs.ToList())
            {
                if (!this.ToList().Select(x => x.IP).ToList().Contains(item.Server_IP))
                {
                    WPF_MVVM.TSQLs.Remove(item);
                }
            }
        }
        #endregion 行為
    }

    /// <summary>
    /// SQL Server 連線組態
    /// </summary>
    public class MSSQL : ViewModelBase
    {
        #region 屬性
        public string Title
        {
            get => Title_;
            set
            {
                Title_ = value;
                OnPropertyChanged();
            }
        }
        private string Title_;

        public string IP
        {
            get => IP_;
            set
            {
                IP_ = value;
                OnPropertyChanged();
            }
        }
        private string IP_;

        public string User
        {
            get => User_;
            set
            {
                User_ = value;
                OnPropertyChanged();
            }
        }
        private string User_;

        public string Password
        {
            get => Password_;
            set
            {
                Password_ = value;
                OnPropertyChanged();
            }
        }
        private string Password_;

        public string def_Schema 
        {
            get => def_Schema_;
            set
            {
                def_Schema_ = value;
                OnPropertyChanged();
            }
        }
        private string def_Schema_;

        [JsonIgnore]
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

        public List<Schema> Schemas { get => Schemas_; }
        private List<Schema> Schemas_;
        #endregion 屬性

        #region 行為
        public SqlConnection Connecting()
        {
            Server.MSSQL.Setting(IP, User, Password, def_Schema);
            return Server.MSSQL.Connecting();
        }

        public List<Schema> GetSchemas()
        {
            try
            {
                Schemas_ = new List<Schema>();
                using (var con = this.Connecting())
                {
                    con.Open();
                    string str = "SELECT * FROM sys.databases where [name] not in ('master', 'model', 'msdb', 'tempdb');";
                    var cmd = new SqlCommand(str, con);
                    var DataReader = cmd.ExecuteReader();
                    while (DataReader.Read())
                    {
                        Schemas_.Add(new Schema(this)
                        {
                            Name = DataReader["name"].ToString()
                        });
                    }
                }
                Schemas_.Sort((x, y) => x.Name.CompareTo(y.Name));
                this.Signal = MVVM.Resources["Green"];
                OnPropertyChanged("Schemas");
                return Schemas;
            }
            catch (Exception ex)
            {
                Schemas_ = new List<Schema>();
                this.Signal = MVVM.Resources["Red"];
                return Schemas;
            }
        }
        #endregion 行為

    }

    /// <summary>
    /// 資料庫 模式
    /// </summary>
    public class Schema : ViewModelBase
    {
        #region 屬性
        public string Name
        {
            get => Name_;
            set
            {
                Name_ = value;
                OnPropertyChanged();
            }
        }
        private string Name_;

        public List<Table> Tables { get => Tables_; }
        private List<Table> Tables_;
        protected MSSQL MSSQL;
        #endregion 屬性

        #region 行為
        public Schema(MSSQL MSSQL)
        {
            this.MSSQL = MSSQL;
            this.Name = Name;

        }

        public List<Table> GetTables()
        {
            _ = Task.Run(() =>
            {
                try
                {
                    WPF_MVVM.MSSQLs.IsLoading = Visibility.Visible;

                    Tables_ = new List<Table>();
                    using (var con = MSSQL.Connecting())
                    {
                        con.Open();
                        string str = $@"select * from INFORMATION_SCHEMA.TABLES where TABLE_CATALOG = '{Name}';";
                        var cmd = new SqlCommand(str, con);
                        var DataReader = cmd.ExecuteReader();
                        while (DataReader.Read())
                        {
                            Tables_.Add(new Table
                            {
                                Name = DataReader["TABLE_SCHEMA"].ToString() + "." + DataReader["TABLE_NAME"].ToString()
                            });
                        }
                    }
                    Tables_.Sort((x, y) => x.Name.CompareTo(y.Name));
                    OnPropertyChanged("Tables");
                }
                catch (Exception ex)
                {
                    Tables_ = new List<Table>();
                    MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
                }
                finally
                {
                    WPF_MVVM.MSSQLs.IsLoading = Visibility.Collapsed;
                }
            });
            return Tables;
        }
        #endregion 行為
    }

    /// <summary>
    /// 表格
    /// </summary>
    public class Table
    {
        public string Name { get; set; }
    }
    #endregion MSSQL 連線


    #region SQL 語法操作物件
    /// <summary>
    /// T-SQL 清單
    /// </summary>
    public class TSQL_List : BindingList<TSQL>
    {
        public TSQL_List()
        {
            foreach (var item in Settings.Default.TSQL_List.ToObject<List<TSQL>>())
            {
                this.Add(item);
            }
            this.ListChanged += SQL_List_ListChanged;
        }

        private void SQL_List_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                TSQL sql = this[e.NewIndex];
                if (string.IsNullOrEmpty(sql.Title))
                {
                    sql.Title = $"MSSQL - {this.Count}";
                }
            }
            Settings.Default.TSQL_List = this.ToJsonString();
        }
    }

    /// <summary>
    /// T-SQL 操作資訊
    /// </summary>
    public class TSQL : ViewModelBase
    {
        #region 屬性
        public string UID { get; }
        /// <summary>
        /// 標題
        /// </summary>
        public string Title
        {
            get => Title_;
            set
            {
                Title_ = value;
                OnPropertyChanged();
            }
        }
        private string Title_;
        /// <summary>
        /// SQL Server 連線
        /// </summary>
        public string Server_IP
        {
            get => Server_IP_;
            set
            {
                Server_IP_ = value;
                OnPropertyChanged();
            }
        }
        private string Server_IP_;
        private MSSQL Server => WPF_MVVM.MSSQLs.ToList().Find(x => x.IP == Server_IP);
        /// <summary>
        /// SQL 語法
        /// </summary>
        public string T_SQL
        {
            get => T_SQL_;
            set
            {
                T_SQL_ = value;
                OnPropertyChanged();
            }
        }
        private string T_SQL_;
        /// <summary>
        /// 執行數據
        /// </summary>
        [JsonIgnore]
        public DataView Data { get; set; } = new DataView();
        /// <summary>
        /// 執行時間
        /// </summary>
        [JsonIgnore]
        public DateTime DateTime
        {
            get => DateTime_;
            set
            {
                DateTime_ = value;
                OnPropertyChanged();
            }
        }
        private DateTime DateTime_;
        /// <summary>
        /// 執行結果
        /// </summary>
        [JsonIgnore]
        public string Execute_Results
        {
            get => Execute_Results_;
            set
            {
                Execute_Results_ = value;
                OnPropertyChanged();
            }
        }
        private string Execute_Results_;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Visibility IsLoading
        {
            get => IsLoading_;
            set
            {
                IsLoading_ = value;
                OnPropertyChanged();
            }
        }
        private Visibility IsLoading_ = Visibility.Collapsed;
        #endregion 屬性

        #region 行為
        public TSQL()
        {
            this.UID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 執行 SQL 語法
        /// </summary>
        /// <returns></returns>
        public void Execute()
        {
            _ = Task.Run(() =>
            {
                try
                {
                    IsLoading = Visibility.Visible;

                    Data = new DataView();
                    if (!string.IsNullOrEmpty(T_SQL) && Server != null)
                    {
                        #region 執行 SQL
                        using (var con = Server.Connecting())
                        {
                            con.Open();
                            Server.Signal = MVVM.Resources["Green"];

                            var cmd = new SqlCommand(T_SQL, con);
                            var DataReader = cmd.ExecuteReader();
                            using (DataTable dt = new DataTable())
                            {
                                dt.Load(DataReader);
                                Data = dt.DefaultView;
                            }
                            Execute_Results = $"Get {Data.Count} rows of data";

                            var Inserter = cmd.ExecuteNonQuery();
                            if (Inserter > -1)
                            {
                                Execute_Results = $"Effected {Inserter} rows of data";
                            }
                        }
                        #endregion 執行 SQL
                    }
                    else if (Server == null)
                    {
                        throw new Exception("找不到連線伺服器");
                    }
                }
                catch (Exception ex)
                {
                    if (ex is SqlException)
                    {
                        Server.Signal = MVVM.Resources["Red"];
                    }
                    Execute_Results = ex.Message;
                }
                finally
                {
                    DateTime = DateTime.Now.Clone();
                    OnPropertyChanged("Data");
                    IsLoading = Visibility.Collapsed;
                }
            });
        }
        #endregion 行為

    }
    #endregion SQL 語法操作物件

}
