using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WPF.ViewModel
{
    public class MoveFile : Mission.Loop
    {
        [Property("讀取路徑")]
        public string Location { get; set; }
        [Property("移動至")]
        public string MoveTo { get; set; }

        public override void Process()
        {
            if (!Directory.Exists(this.Location))
            {
                MVVM.Show(Titel + "任務中讀取路徑不存在");
                this.Terminate();
                return;
            }
            if (!Directory.Exists(this.MoveTo))
            {
                MVVM.Show(Titel + "任務中存放位置不存在");
                this.Terminate();
                return;
            }

            DirectoryInfo directory = new DirectoryInfo(Location);
            if (directory.GetFiles().Length > 0)
            {
                FileInfo file = directory.GetFiles().First();
                string path = File.Exists(this.MoveTo + @"\" + file.Name) ?
                    this.MoveTo + @"\" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + file.Extension :
                    this.MoveTo + @"\" + file.Name;

                if (!IsFileLocked(file.FullName))
                {
                    file.MoveTo(path);
                }
            }
        }


        private bool IsFileLocked(string path)
        {
            try
            {
                using (File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException exception)
            {
                int errorCode = Marshal.GetHRForException(exception) & 65535;
                return errorCode == 32 || errorCode == 33;
            }
            catch (Exception)
            {
                return true;
            }
        }
    }

}
