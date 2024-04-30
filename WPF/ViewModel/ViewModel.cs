using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

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

    }
}
