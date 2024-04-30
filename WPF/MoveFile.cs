using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Properties;
using static Component.ViewModel.Mission;

namespace WPF
{ 
    public class MoveFile : Loop
    {
        public MoveFile()
        {
            this.Titel = "MoveFile";
            this.Cycle_Time = TimeSpan.Parse("00:00:01");
        }    

        public override void Process()
        {
            
        }
    }


}
