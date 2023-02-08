using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Lib.Models
{
    public class ConfigFileModel // This Model is used to collect the path of the log files
    {
        public string Daily_log_emplacement { get; set; }
        public string Status_log_emplacement { get; set; }
    }
}
