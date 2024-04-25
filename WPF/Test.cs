using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using API;
using ToolBox.ExtensionMethods;
using Newtonsoft.Json;

namespace TestCode
{
    /// <summary>
    /// 客戶序號
    /// </summary>
    public class Customer_Code : Base
    {
        #region 屬性
        /// <summary>
        /// 客戶序號項目
        /// </summary>
        public string Case;
        /// <summary>
        /// 客戶
        /// </summary>
        public string Customer;
        /// <summary>
        /// 備註
        /// </summary>
        public string Remark;
        /// <summary>
        /// 序號清單
        /// </summary>
        public List<Serial_Code> Codes = new List<Serial_Code>();
        /// <summary>
        /// 是否已配發
        /// </summary>
        public bool IsReceived;
        /// <summary>
        /// 資料更新時間
        /// </summary>
        public DateTime Update_Time;
        #endregion 屬性


        #region 行為
        public bool Save()
        {
            try
            {
                List<string> list = new List<string>();
                foreach (var item in this.Codes)
                {
                    list.Add(item.Value);
                }

                using (var con = API.Server.MSSQL.Connecting())
                {
                    con.Open();
                    string str = $@"Insert into [TestDB].[dbo].[Customer_Code]([Case], [Customer], [Remark], [Codes], [IsReceived], [Update_Time]) 
                                Select
                                    '{this.Case}' as [Case],
                                    '{this.Customer}' as [Customer],
                                    '{this.Remark}' as [Remark],
                                    '{JsonConvert.SerializeObject(this.Codes)}' as [Codes],
                                    '{this.IsReceived}' as [IsReceived],
                                    GETDATE() as [Update_Time]
                                Where NOT EXISTS (
                                    Select TOP(1) * from
                                    (SELECT * FROM [TestDB].[dbo].[Customer_Code] where [Case] = '{this.Case}') as t
                                    Outer Apply OPENJSON(t.[Codes], '$')
                                    With(
	                                    [Value] varchar(50) '$.Value'
                                    ) as t2
                                    Where t2.[Value] in ('{string.Join("','", list.ToArray())}')
                                )";

                    var cmd = new SqlCommand(str, con);
                    var Qty = cmd.ExecuteNonQuery();
                    if (Qty > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool BulkInsert(List<Customer_Code> List)
        {
            try
            {
                using (var con = API.Server.MSSQL.Connecting())
                {
                    con.Open();
                    string str = $@"DROP TABLE IF EXISTS ##Codes;
                                    Select [Case], [Key], [Value] Into ##Codes 
                                    From [TestDB].[dbo].[Customer_Code]
                                    Cross Apply OPENJSON([Codes], '$')
	                                    With (
	                                    [Key] varchar(50) '$.Key',
	                                    [Value] varchar(50) '$.Value'
                                    )
                                    where [Case] = '{List[0].Case}'
                                    Create Index index1 on tempdb.##Codes([Value]);";

                    var cmd = new SqlCommand(str, con);
                    var Qty = cmd.ExecuteNonQuery();

                    foreach (var item in List)
                    {
                        List<string> list = new List<string>();
                        foreach (var code in item.Codes)
                        {
                            list.Add(code.Value);
                        }

                        str = $@"Insert into [TestDB].[dbo].[Customer_Code]([Case], [Customer], [Remark], [Codes], [IsReceived], [Update_Time]) 
                                Select
                                    '{item.Case}' as [Case],
                                    '{item.Customer}' as [Customer],
                                    '{item.Remark}' as [Remark],
                                    '{JsonConvert.SerializeObject(item.Codes)}' as [Codes],
                                    '{item.IsReceived}' as [IsReceived],
                                    GETDATE() as [Update_Time]
                                Where NOT EXISTS (
                                    Select TOP(1) * from tempdb.##Codes
                                    Where [Value] in ('{string.Join("','", list.ToArray())}')
                                )";

                        cmd = new SqlCommand(str, con);
                        Qty = cmd.ExecuteNonQuery();
                        if (Qty > 0)
                        {

                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public Customer_Code Receive(string Case)
        {
            try
            {
                using (var con = API.Server.MSSQL.Connecting())
                {
                    con.Open();
                    string str = $@"Select TOP(1) * from [TestDB].[dbo].[Customer_Code] 
                                    Where [IsReceived] = '{bool.FalseString}' and [Case] = '{Case}';";

                    var cmd = new SqlCommand(str, con);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        this.ID = reader["ID"].ToString();
                        this.Name = reader["Customer"].ToString() + "-" + reader["Case"].ToString() + " 序號";
                        this.Case = reader["Case"].ToString();
                        this.Customer = reader["Customer"].ToString();
                        this.Remark = reader["Remark"].ToString();
                        this.Codes = JsonConvert.DeserializeObject<List<Serial_Code>>(reader["Codes"].ToString());
                        this.IsReceived = reader["IsReceived"].ToString().ToBool();
                        this.Update_Time = DateTime.Parse(reader["Update_Time"].ToString());
                    }
                    reader.Close();
                    //if (!string.IsNullOrEmpty(this.ID))
                    //{
                    //    str = $@"Update [TestDB].[dbo].[Customer_Code]
                    //            Set [IsReceived] = '{bool.TrueString}',
                    //            [Update_Time] = '{DateTime.Now.ToCommonly()}'
                    //            Where [ID] = '{this.ID}';";

                    //    cmd = new SqlCommand(str, con);
                    //    var Qty = cmd.ExecuteNonQuery();
                    //    if (Qty > 0)
                    //    {
                    //        return this;
                    //    }
                    //}
                    return this;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataSet GetDataSet(string Case)
        {
            try
            {
                DataSet ds = new DataSet();
                using (var con = API.Server.MSSQL.Connecting())
                {
                    con.Open();
                    string str = $@"Select TOP(1) * from [TestDB].[dbo].[Customer_Code] 
                                    Where [IsReceived] = '{bool.FalseString}' and [Case] = '{Case}';";

                    var cmd = new SqlCommand(str, con);
                    var reader = cmd.ExecuteReader();
                    DataTable Info = new DataTable();
                    Info.TableName = "Info";
                    Info.Load(reader);
                    ds.Tables.Add(Info);

                    DataTable Codes = JsonConvert.DeserializeObject<DataTable>(Info.Rows[0]["Codes"].ToString());
                    Codes.TableName = "Codes";
                    ds.Tables.Add(Codes);

                    ds.Tables[0].Columns.Remove(ds.Tables[0].Columns["Codes"]);
                }
                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion 行為
    }


    public class Serial_Code
    {
        /// <summary>
        /// 序號名稱
        /// </summary>
        public string Key;
        /// <summary>
        /// 序號
        /// </summary>
        public string Value;
    }


}
