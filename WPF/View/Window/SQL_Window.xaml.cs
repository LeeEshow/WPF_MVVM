using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF.View.Controller;
using WPF.ViewModel;
using Table = WPF.ViewModel.Table;

namespace WPF.View.Window
{
    /// <summary>
    /// SQL_Window.xaml 的互動邏輯
    /// </summary>
    public partial class SQL_Window : MetroWindow
    {
        public SQL_Window()
        {
            InitializeComponent();
            this.DataContext = this;
            tab_SQL.ItemsSource = WPF_MVVM.TSQLs;
            tv_Server.ItemsSource = WPF_MVVM.MSSQLs;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (tab_SQL.Items.Count > 0)
            {
                tab_SQL.SelectedIndex = 0;
            }
        }

        #region 新增/移除 分頁
        private void Add_TSQL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectServer != null)
                {
                    TSQL sql = new TSQL
                    {
                        Server = SelectServer,
                        Title = SelectServer.IP + "-" + DateTime.Now.ToString("HHmmss"),
                        DateTime = DateTime.Now,
                        Execute_Results = "New Query"
                    };

                    WPF_MVVM.TSQLs.Add(sql);
                    tab_SQL.Items.Refresh();

                    tab_SQL.SelectedIndex = tab_SQL.Items.Count - 1;
                }
                else
                {
                    MessageBox.Show("選擇伺服器!!");
                }
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }

        private void Delete_TSQL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                var item = WPF_MVVM.TSQLs.ToList().Find(x => x.UID == btn.ToolTip.ToString());
                if (item != null)
                {
                    WPF_MVVM.TSQLs.Remove(item);
                    tab_SQL.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }
        #endregion 新增/移除 分頁

        #region 執行
        public TSQL SelectItem { get; set; }
        private void Execute_TSQL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectItem != null)
                {
                    SelectItem.Execute();
                }
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }

        private void Execute_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                Execute_TSQL_Click(null, new RoutedEventArgs());
            }
        }
        #endregion 執行

        #region 左邊 Treeview
        public MSSQL SelectServer { get; set; }
        public Schema SelectSchema { get; set; }
        public Table SelectTable { get; set; }
        private void tv_Server_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem tvi = e.OriginalSource as TreeViewItem;
                switch (tvi.DataContext.GetType().Name)
                {
                    case "MSSQL":
                        MSSQL sql = (MSSQL)tvi.DataContext;
                        SelectServer = sql;
                        if (sql.Schemas == null)
                        {
                            sql.GetSchemas();
                        }
                        break;

                    case "Schema":
                        Schema schema = (Schema)tvi.DataContext;
                        SelectSchema = schema;
                        if (schema.Tables == null)
                        {
                            schema.GetTables();
                        }
                        break;

                    case "Table":
                        Table table = (Table)tvi.DataContext;
                        SelectTable = table;
                        break;
                }
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }

        private void Select1000_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectServer != null)
                {
                    TSQL sql = new TSQL
                    {
                        Server = SelectServer,
                        Title = SelectServer.IP + "-" + DateTime.Now.ToString("HHmmss"),
                        DateTime = DateTime.Now,
                        Execute_Results = "New Query",
                        T_SQL = $@"Select Top(1000) * From {SelectSchema.Name}.{SelectTable.Name};"
                    };

                    WPF_MVVM.TSQLs.Add(sql);
                    tab_SQL.Items.Refresh();

                    tab_SQL.SelectedIndex = tab_SQL.Items.Count - 1;
                    Execute_TSQL_Click(null, new RoutedEventArgs());
                }
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }
        
        private void TreeViewItem_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)GetItem<TreeViewItem>(e.OriginalSource as DependencyObject);
            if (tvi != null)
            {
                tvi.Focus();
                tvi.IsSelected = true;
            }
        }
        private static DependencyObject GetItem<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
            {
                source = VisualTreeHelper.GetParent(source);
            }
            return source;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                DockPanel dp = (DockPanel)btn.Parent;
                var item = WPF_MVVM.MSSQLs.ToList().Find(x => x.IP == dp.ToolTip.ToString());
                if (item != null)
                {
                    item.GetSchemas();
                    SelectServer = item;
                    tv_Server.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }
        #endregion 左邊 Treeview

        #region 連線編輯
        private void Add_SQL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WPF_MVVM.MSSQLs.EditItem = new MSSQL();
                SQL_Config config = new SQL_Config();
                config.ShowDialog();
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }

        private void Delete_SQL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                DockPanel dp = (DockPanel)btn.Parent;
                var item = WPF_MVVM.MSSQLs.ToList().Find(x => x.IP == dp.ToolTip.ToString());
                if (item != null)
                {
                    WPF_MVVM.MSSQLs.Remove(item);
                }
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }

        private void Edit_SQL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                DockPanel dp = (DockPanel)btn.Parent;
                var item = WPF_MVVM.MSSQLs.ToList().Find(x => x.IP == dp.ToolTip.ToString());
                if (item != null)
                {
                    WPF_MVVM.MSSQLs.EditItem = item;
                    SQL_Config config = new SQL_Config();
                    config.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MVVM.ExceptionEvent(MethodBase.GetCurrentMethod(), ex);
            }
        }
        #endregion 連線編輯
    }
}
