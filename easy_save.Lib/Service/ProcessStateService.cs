using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Lib.Service
{
    public class ProcessStateService
    {
        public bool GetProcessState(string processName)
        {

            Process[] process = Process.GetProcessesByName(processName);

            return process.Length > 0;
        }

    }
}
