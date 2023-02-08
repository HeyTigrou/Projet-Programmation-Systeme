using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Lib.Models
{
    // This Model is used to collect the path of the log files
    public class ConfigFileModel 
    {
        public string Daily_log_emplacement { get; set; }
        public string Status_log_emplacement { get; set; }
        public int Max_number_of_save { get; set; }
    }
}
