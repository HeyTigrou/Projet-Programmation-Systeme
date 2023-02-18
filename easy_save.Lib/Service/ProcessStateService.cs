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
            bool isRunning = false;

            Process[] process = Process.GetProcessesByName(processName);
            if (process.Length > 0)
            {
                isRunning = true;
            }
            return isRunning;
        }
    }
}
