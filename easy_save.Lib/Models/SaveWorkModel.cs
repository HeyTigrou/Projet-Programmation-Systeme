using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace easy_save.Lib.Models
{
    /// <summary>
    /// This Model is used to manage the save works
    /// </summary>
    public class SaveWorkModel 
    {
        public string Name { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public int SaveType { get; set; }

        [JsonIgnore]
        public string Progression { get; set; } = "";
    }
}
