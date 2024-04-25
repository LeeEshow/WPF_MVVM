using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF
{
    /// <summary>
    /// MessageShow.xaml 的互動邏輯
    /// </summary>
    public partial class MessageShow : UserControl
    {
        public MessageShow()
        {
            InitializeComponent();
            VM.ShowEvent += Setup_ShowEvent;
        }

        private void Setup_ShowEvent(string Message)
        {
            try
            {
                RichTextBox(Message);
            }
            catch { }
        }

        private delegate void RichTextBoxCallBack(string str);
        private void RichTextBox(string str)
        {
            if (!Dispatcher.CheckAccess())
            {
                RichTextBoxCallBack myUIpdate = new RichTextBoxCallBack(RichTextBox);
                Dispatcher.Invoke(myUIpdate, str);
            }
            else
            {
                Run Newrun = new Run(Environment.NewLine + $"【{DateTime.Now.ToString("MM/dd HH:mm:ss")}】");
                Newrun.FontSize = 14;
                txt_message_Paragraph.Inlines.Add(Newrun);

                Newrun = new Run( $" {str}");
                Newrun.FontSize = 14;
                Newrun.Foreground = Brushes.LightGray;
                txt_message_Paragraph.Inlines.Add(Newrun);
            }
        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                rtb_msage.ScrollToEnd();
                if (txt_message_Paragraph != null)
                {
                    if (txt_message_Paragraph.Inlines.Count > 100)
                    {
                        txt_message_Paragraph.Inlines.Remove(txt_message_Paragraph.Inlines.FirstInline);
                    }
                }
            }
            catch { }
        }
    }
}
