using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Lib.Models
{
    // This Model is used to create the daily log file
    public class DailyLoggerModel 
    {
        public string Name { set; get; }
        public string SourceFilePath { set; get; }
        public string TargetFilePath { set; get; }
        public long Filesize { set; get; }
        public TimeSpan FileTransferTime { set; get; }
        public string Time { set; get; }
    }
}
