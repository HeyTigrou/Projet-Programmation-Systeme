using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace easy_save.Lib.Models
{
    public enum StateTypes
    {
        Running,
        Done
    }

    /// <summary>
    /// This Model is used to create the state log
    /// </summary>
    public class StateLoggerModel 
    {
        
        public string Name { set; get; }
        public string SourceFilePath { set; get; }
        public string TargetFilePath { set; get; }
        public string State { set; get; }
        public int TotalFileToCopy { set; get; }
        public long TotalFileSize { set; get; }
        public long NbFilesLeft { set; get; }
        public float Progression { get => (TotalFileToCopy!=0)? (float)(TotalFileToCopy - NbFilesLeft) / TotalFileToCopy : 0 ; }
    }
}
