using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using WPF.Properties;
using WPF.ViewModel;

namespace WPF.ViewModel
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
}

// --------------------------------------------------------------------------------
/// <summary>
/// 靜態宣告所有介面【共用】物件，像跨界面傳遞值或全域變數的意思。
/// </summary>
public static class WPF_MVVM
{
    // ViewModel 方式傳遞 
    public static Mission.Manager Manager = new Mission.Manager();

    public static MQTT MQTT = new MQTT();

}


// --------------------------------------------------------------------------------
/// <summary>
/// 靜態擴充
/// </summary>
public static class ExtensionMethods
{
    [DllImport("user32.dll")]
    static extern IntPtr FindWindow(string strClass, string window);
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetForegroundWindow(IntPtr ptr);
    /// <summary>
    /// 開啟視窗，僅開啟一個若重複執行將視窗至前。
    /// </summary>
    /// <param name="window"></param>
    public static void ShowOnly(this Window window)
    {
        IntPtr ptr = FindWindow(null, window.Title);
        if (ptr == IntPtr.Zero)
        {
            window.Show();
        }
        else
        {
            SetForegroundWindow(ptr);
        }
    }

}
