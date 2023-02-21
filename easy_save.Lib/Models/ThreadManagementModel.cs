using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Lib.Models
{
    public class ThreadManagementModel
    {
        public bool QuitThread { get; set; } = false;
        public ManualResetEvent ResetEvent = new ManualResetEvent(false);
    }
}
