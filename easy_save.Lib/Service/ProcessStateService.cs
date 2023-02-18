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
        /// <summary>
        /// This method check if the process is running, returns true if it is.
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public bool GetProcessState(string processName)
        {

            Process[] process = Process.GetProcessesByName(processName);

            return process.Length > 0;
        }

    }
}
